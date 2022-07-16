using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net.WebSockets;
using System.Text;

namespace RmqChat.UI.Pages
{
    public class RoomModel : PageModel
    {
        public async Task Connect()
        {
            using var ws = new ClientWebSocket();
            var _roomName = Request.Form["RoomName"];
            var user = User.Identity?.Name ?? "annonimous";
            var connectInfo = $"{_roomName}:{user}";
            await ws.ConnectAsync(new Uri($"ws://localhost:5107/connect/{connectInfo}"), CancellationToken.None);

            _ = RegisterToReceiveMessagesAsync(ws);
        }

        private static async Task RegisterToReceiveMessagesAsync(ClientWebSocket ws)
        {
            var buffer = new byte[256];
            while (ws.State == WebSocketState.Open)
            {
                var result = await ws.ReceiveAsync(buffer, CancellationToken.None);

                if (result.MessageType == WebSocketMessageType.Close)
                    await ws.CloseAsync(WebSocketCloseStatus.NormalClosure, null, CancellationToken.None);
                else
                    Console.WriteLine(Encoding.ASCII.GetString(buffer, 0, result.Count));
            }
        }
    }
}
