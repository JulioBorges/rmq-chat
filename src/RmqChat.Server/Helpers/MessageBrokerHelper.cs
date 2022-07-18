using RabbitMQ.Client;
using RmqChat.Protocol.Messaging;
using RmqChat.Server.Consumers;

namespace RmqChat.Server.Helpers
{
    public static class MessageBrokerHelper
    {
        private const string exchangeName = "rmqChatTopic-messages";

        public static IConnection GetConnection(string messagingHostName)
        {
            var factory = new ConnectionFactory() { HostName = messagingHostName };
            return factory.CreateConnection();
        }

        public static IModel InstantiateExchangeAndTopic(IConnection connection)
        {
            var channel = connection.CreateModel();

            channel.ExchangeDeclare(exchange: exchangeName, type: "topic");
            return channel;
        }

        public static IModel RegisterConsumerOnTopic<TConsumer>(this IModel channel, 
            TConsumer consumer)
            where TConsumer : BaseRmqConsumer
        {
            var queueName = channel.QueueDeclare().QueueName;

            channel.QueueBind(queue: queueName,
                                  exchange: exchangeName,
                                  routingKey: consumer.RoutingKey);

            channel.BasicConsume(queue: queueName,
                                 autoAck: true,
                                 consumer: consumer);
            return channel;
        }

        public static IModel SendDataToExchange<T>(this IModel channel, T data, string routingKey)
            where T: class
        {
            var body = data.SerializeData();
            channel.BasicPublish(exchange: exchangeName,
                                 routingKey: routingKey,
                                 basicProperties: null,
                                 body: body);
            return channel;
        }

        public static void SendMessageToBroker(string messagingHostName, Message message)
        {
            using var connection = MessageBrokerHelper.GetConnection(messagingHostName);
            using var channel = MessageBrokerHelper.InstantiateExchangeAndTopic(connection);

            channel.SendDataToExchange(message, "message");
        }

        public static void SendCommandToBroker(string messagingHostName, Command command)
        {
            using var connection = MessageBrokerHelper.GetConnection(messagingHostName);
            using var channel = MessageBrokerHelper.InstantiateExchangeAndTopic(connection);

            channel.SendDataToExchange(command, "command");
        }
    }
}
