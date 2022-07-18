using Microsoft.AspNetCore.SignalR;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RmqChat.Interpreters;
using RmqChat.Protocol.Messaging;
using RmqChat.Server.Helpers;
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
            Command command = args.Body.ToArray().DeserializeData<Command>()!;

            if (command == null)
                return;

            var interpreter = InterpreterServiceLocator.GetCommandInterpreter(command.CommandText);

            if (interpreter == null)
                return;

            await interpreter.InterpretCommandAsync(command, async (to, msg) =>
            {
                await _hubContext.Clients.Group(to).SendAsync("ReceiveMessage", InterpreterServiceLocator.BotName, msg);
            });
        }
    }
}