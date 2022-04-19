using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.JSInterop;

namespace ChatAppSignalR.Client.Pages
{
    public partial class Index : IAsyncDisposable
    {

        private HubConnection? hubConnection;
        private string AllMessages = string.Empty;
        private string Username = string.Empty;
        private string Message = string.Empty;
        ElementReference TextAreRef;

        [Inject]
        public NavigationManager Navigation { get; set; } = null!;

        [Inject]
        public IJSRuntime JsRuntime { get; set; }

        private async Task Connect()
        {
            hubConnection = new HubConnectionBuilder()
            .WithUrl(NavigationManager.ToAbsoluteUri($"/chathub?username={Username}"))
            .Build();

            hubConnection.On<string, string>("receive", (user, message) =>
            {
                var msg = $"{(string.IsNullOrEmpty(user) ? "" : user + ": ")}{message}";
                AllMessages += msg + "\n";
                JsRuntime.InvokeVoidAsync("scrollToBottom", TextAreRef);
                StateHasChanged();
            });

            await hubConnection.StartAsync();
        }

        private async Task Send()
        {
            if (hubConnection != null)
            {
                await hubConnection.SendAsync("AddMessage", Username, Message);
                Message = string.Empty;
            }
        }

        private async Task HandleInput(KeyboardEventArgs args)
        {
            if (args.Key.Equals("Enter"))
            {
                await Send();
            }
        }

        public bool IsConnected => hubConnection?.State == HubConnectionState.Connected;

        public async ValueTask DisposeAsync()
        {
            if (hubConnection != null)
            {
                await hubConnection.DisposeAsync();
            }
        }
    }
}
