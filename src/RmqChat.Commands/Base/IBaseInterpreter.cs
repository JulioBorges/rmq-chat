using RmqChat.Protocol.Messaging;

namespace RmqChat.Interpreters.Base
{
    public interface IBaseInterpreter
    {
        Task InterpretCommandAsync(Command command, Action<string, string> replyMessageAction);
    }
}
