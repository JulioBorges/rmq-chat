using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using RmqChat.Data;
using RmqChat.Server.Configuration;
using RmqChat.Server.Consumers;
using RmqChat.Server.Helpers;
using RmqChat.UI.Hubs;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));
builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = false)
    .AddEntityFrameworkStores<ApplicationDbContext>();

var serverConfig = builder.Configuration.GetSection("ServerConfiguration").Get<ServerConfiguration>();
builder.Services.AddSingleton(serverConfig);

builder.Services.AddSignalR();
builder.Services.AddCors();

using var connection = MessageBrokerHelper.GetConnection(serverConfig.MessagingHostName);
using var _channel = MessageBrokerHelper.InstantiateExchangeAndTopic(connection);
builder.Services.AddSingleton(_channel);

builder.Services.AddSingleton<MessageConsumer>();
builder.Services.AddSingleton<CommandConsumer>();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    db.Database.Migrate();
}

app.UseCors(builder =>
{
    builder.WithOrigins("https://localhost:7221", "http://localhost:5221")
        .AllowAnyHeader()
        .AllowAnyMethod()
        .AllowCredentials();
});

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.UseEndpoints(endpoints =>
{
    endpoints.MapHub<RmqChatHub>("/rmqchathub");
});

_channel.RegisterConsumerOnTopic(app.Services.GetService<MessageConsumer>()!);
_channel.RegisterConsumerOnTopic(app.Services.GetService<CommandConsumer>()!);

await app.RunAsync();