using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace RmqChat.Server.Consumers
{
    public abstract class BaseRmqConsumer : EventingBasicConsumer
    {
        public string RoutingKey { get; private set; }
        public bool Durable { get; set; }

        protected BaseRmqConsumer(IModel model, string routingKey) : base(model)
        {
            RoutingKey = routingKey;
            Durable = false;
            Received += (model, ea) =>
            {
                if (ea.RoutingKey == RoutingKey)
                {
                    OnReceivedMessageAsync(model, ea);
                }
            };
        }

        protected abstract Task OnReceivedMessageAsync(object? model, BasicDeliverEventArgs args);
    }
}
