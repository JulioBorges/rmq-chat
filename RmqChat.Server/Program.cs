using System.Net;
using System.Net.WebSockets;
using System.Text;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();
app.UseWebSockets();

app.Map("/connect/{connectInfo}", async context =>
{
    if (!context.WebSockets.IsWebSocketRequest)
        context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
    else
    {
        using var webSocket = await context.WebSockets.AcceptWebSocketAsync();
        var connectInfo = context.GetRouteValue("connectInfo").ToString();

        var roomName = connectInfo.Split(':')[0];
        var user = connectInfo.Split(':')[1];
        Console.WriteLine(roomName);
        Console.WriteLine(user);
    }
});

await app.RunAsync();