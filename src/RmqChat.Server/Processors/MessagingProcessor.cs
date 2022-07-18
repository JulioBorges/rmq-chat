using Microsoft.AspNetCore.SignalR;
using RmqChat.Interpreters;
using RmqChat.Protocol.Messaging;
using RmqChat.Server.Configuration;
using RmqChat.Server.Helpers;

namespace RmqChat.Server.Processors
{
    public class MessagingProcessor
    {
        private readonly ServerConfiguration _serverConfiguration;

        public MessagingProcessor(ServerConfiguration serverConfiguration)
        {
            _serverConfiguration = serverConfiguration;
        }

        public void ProcessMessage(string user, string message)
        {
            if (message.StartsWith("/"))
            {
                try
                {
                    var command = BuildCommand(user, message);
                    MessageBrokerHelper.SendCommandToBroker(_serverConfiguration.MessagingHostName, command);
                }
                catch
                {
                    var msg = BuildMessage(InterpreterServiceLocator.BotName, $"Command [{message}] invalid !");
                    msg.To = user;

                    MessageBrokerHelper.SendMessageToBroker(_serverConfiguration.MessagingHostName, msg);
                }
            }
            else
            {
                var msg = BuildMessage(user, message);
                MessageBrokerHelper.SendMessageToBroker(_serverConfiguration.MessagingHostName, msg);
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
