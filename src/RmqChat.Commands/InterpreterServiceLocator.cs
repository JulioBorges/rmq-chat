using RmqChat.Interpreters.Base;
using RmqChat.Interpreters.StockBot;

namespace RmqChat.Interpreters
{
    public static class InterpreterServiceLocator
    {
        public static string BotName = "RmqChat BOT";
        
        public static IBaseInterpreter? GetCommandInterpreter(string commandText)
        {
            switch(commandText)
            {
                case "/stock":
                    {
                        return new StockBotInterpreter();
                    }
                default:
                    return null;
            }
        }
    }
}
