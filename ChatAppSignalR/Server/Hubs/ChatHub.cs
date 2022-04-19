using Microsoft.AspNetCore.SignalR;

namespace ChatAppSignalR.Server.Hubs
{
    public class ChatHub : Hub
    {
        private static Dictionary<string, string> Users = new Dictionary<string, string>();

        public override async Task OnConnectedAsync()
        {
            string username = Context.GetHttpContext().Request.Query["Username"];
            Users.Add(Context.ConnectionId, username);
            await AddMessage(string.Empty, $"{username} entered the chat!");
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? e)
        {
            string username = Users.FirstOrDefault(u => u.Key == Context.ConnectionId).Value;
            await AddMessage(string.Empty, $"{username} left the chat.");
        }
        


        public async Task AddMessage(string user, string message)
        {
            await Clients.All.SendAsync("receive", user, message);
        }
    }
}
