using Microsoft.AspNetCore.SignalR;
using RmqChat.Server.Configuration;
using RmqChat.Server.Processors;

namespace RmqChat.UI.Hubs
{
    public class RmqChatHub : Hub
    {
        private readonly ServerConfiguration _serverConfiguration;

        public RmqChatHub(ServerConfiguration serverConfiguration)
        {
            _serverConfiguration = serverConfiguration;
        }

        public async Task ConnectUser(string user)
        {
            await Groups.AddToGroupAsync(Context?.ConnectionId ?? "", user);
            await Clients.All.SendAsync("UserConnected", user);
        }

        public Task ProcessMessage(string user, string message)
        {
            new MessagingProcessor(_serverConfiguration.MessagingHostName!).ProcessMessage(user, message);
            return Task.CompletedTask;
        }

        public async Task SendMessageToSpecificClient(string sender, string receiver, string message)
        {
            await Clients.Group(receiver).SendAsync("ReceiveMessage", sender, message);
        }

        public async Task SendMessageToAllClients(string user, string message)
        {
            await Clients.All.SendAsync("ReceiveMessage", user, message);
        }
    }
}
