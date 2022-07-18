using RmqChat.Interpreters.Base;
using RmqChat.Interpreters.StockBot.Mappings;
using RmqChat.Interpreters.StockBot.Model;
using RmqChat.Protocol.Messaging;
using System.Text;
using TinyCsvParser;

namespace RmqChat.Interpreters.StockBot
{
    public class StockBotInterpreter : IBaseInterpreter
    {
        private static readonly HttpClient httpClient = new();
        private static readonly CsvParser<StockOption> csvParser = new(
            new CsvParserOptions(true, ','),
            new StockOptionMapping());

        public async Task InterpretCommandAsync(Command command, Action<string, string> replyMessageAction)
        {
            try
            {
                var str = await GetResponseStreamAsync(command.CommandArgs);
                var stockResult = csvParser.ReadFromStream(str, Encoding.UTF8).ToArray();
                var stock = stockResult.FirstOrDefault();
                if (stock != null)
                {
                    var data = stock.Result;
                    replyMessageAction(command.From!, $"{data.Symbol} is ${data.Close} per share");
                }
            }
            catch
            {
                replyMessageAction(command.From!, "Error processing command");
            }
        }

        private static async Task<Stream> GetResponseStreamAsync(string stock)
        {
            var uri = $"https://stooq.com/q/l/?s={stock}&f=sd2t2ohlcv&h&e=csv";

            var response = await httpClient.GetAsync(uri);

            if (!response.IsSuccessStatusCode)
                throw new Exception("Error, status: " + response.StatusCode);

            return await response.Content.ReadAsStreamAsync();
        }
    }
}
