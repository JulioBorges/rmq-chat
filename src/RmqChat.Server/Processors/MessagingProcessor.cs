using Microsoft.AspNetCore.SignalR;
using RmqChat.Protocol.Messaging;
using RmqChat.Server.Helpers;

namespace RmqChat.Server.Processors
{
    public class MessagingProcessor
    {
        private readonly string _messagingHostName;

        public MessagingProcessor(string messagingHostName)
        {
            _messagingHostName = messagingHostName;
        }

        public void ProcessMessage(string user, string message)
        {
            if (message.StartsWith("/"))
            {
                try
                {
                    var command = BuildCommand(user, message);
                    MessageBrokerHelper.SendCommandToBroker(_messagingHostName, command);
                }
                catch
                {
                    var msg = BuildMessage("RmqChat", $"Command [{message}] invalid !");
                    msg.To = user;

                    MessageBrokerHelper.SendMessageToBroker(_messagingHostName, msg);
                }
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
                CommandArgs = message.Split('=')[1]
            };

        private static Message BuildMessage(string user, string message) =>
            new()
            {
                From = user,
                Resource = message
            };
    }
}
