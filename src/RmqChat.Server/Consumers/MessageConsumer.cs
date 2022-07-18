using Microsoft.AspNetCore.SignalR;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RmqChat.Protocol.Messaging;
using RmqChat.Server.Helpers;
using RmqChat.UI.Hubs;

namespace RmqChat.Server.Consumers
{
    public class MessageConsumer : BaseRmqConsumer
    {
        private readonly IHubContext<RmqChatHub> _hubContext;

        public MessageConsumer(IModel model, IHubContext<RmqChatHub> hubContext) : base(model, "message")
        {
            _hubContext = hubContext;
            Durable = true;
        }

        protected override async Task OnReceivedMessageAsync(object? model, BasicDeliverEventArgs args)
        {
            Message message = args.Body.ToArray().DeserializeData<Message>()!;

            if (message == null)
                return;

            if (string.IsNullOrEmpty(message.To))
            {
                await _hubContext.Clients.All
                    .SendAsync("ReceiveMessage", message.From, message.Resource.ToString());
            }
            else
            {
                await _hubContext.Clients.Group(message.To).SendAsync("ReceiveMessage", message.From, message.Resource.ToString());
            }
        }
    }
}
