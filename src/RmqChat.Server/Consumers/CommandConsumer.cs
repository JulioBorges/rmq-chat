using Microsoft.AspNetCore.SignalR;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RmqChat.UI.Hubs;

namespace RmqChat.Server.Consumers
{
    public class CommandConsumer : BaseRmqConsumer
    {
        private readonly IHubContext<RmqChatHub> _hubContext;

        public CommandConsumer(IModel model, IHubContext<RmqChatHub> hubContext) : base(model, "command")
        {
            _hubContext = hubContext;
        }

        protected override async Task OnReceivedMessageAsync(object? model, BasicDeliverEventArgs args)
        {
            Console.WriteLine("Command received: " + args.Body);
        }
    }
}