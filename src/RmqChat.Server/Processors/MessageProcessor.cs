using Microsoft.AspNetCore.SignalR;
using RmqChat.Protocol.Messaging;
using RmqChat.Server.Helpers;

namespace RmqChat.Server.Processors
{
    public class MessageProcessor
    {
        private readonly string _messagingHostName;

        public MessageProcessor(string messagingHostName)
        {
            _messagingHostName = messagingHostName;
        }

        public void ProcessMessage(string user, string message)
        {
            if (message.StartsWith("/"))
            {
                var command = BuildCommand(user, message);
                MessageBrokerHelper.SendCommandToBroker(_messagingHostName, command);
            }
            else
            {
                var msg = BuildMessage(user, message);
                MessageBrokerHelper.SendMessageToBroker(_messagingHostName, msg);
            }
        }

        private static Command BuildCommand(string user, string message) =>
            new ()
            {
                From = user,
                CommandText = message.Split('=')[0],
                ComandArgs = message.Split('=')[1]
            };

        private static Message BuildMessage(string user, string message) =>
            new()
            {
                From = user,
                Resource = message
            };
    }
}
