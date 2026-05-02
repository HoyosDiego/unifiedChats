using Microsoft.EntityFrameworkCore;
using UnifiedChat.Api.Hubs;
using UnifiedChat.Api.Services;
using UnifiedChat.Domain.Interfaces;
using UnifiedChat.Infrastructure.Messaging;
using UnifiedChat.Infrastructure.Persistence;
using UnifiedChat.Infrastructure.Workers;

var builder = WebApplication.CreateBuilder(new WebApplicationOptions
{
    Args = args,
    ContentRootPath = AppContext.BaseDirectory
});

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseSqlServer(connectionString);
});

builder.Host.UseWindowsService();

builder.Services.AddWindowsService(options =>
{
    options.ServiceName = "UnifiedChatService";
});

builder.WebHost.ConfigureKestrel(options =>
{
    options.ListenAnyIP(5000);
});

builder.Services.AddControllers();
builder.Services.AddSignalR();

builder.Services.AddSingleton<IMessageBus, InMemoryMessageBus>();
builder.Services.AddHostedService<TikTokChatWorker>();
builder.Services.AddHostedService<TwitchChatWorker>();
builder.Services.AddHostedService<MessageDispatcher>();

var app = builder.Build();

app.UseRouting();

app.UseCors(policy =>
{
    policy.AllowAnyHeader()
          .AllowAnyMethod()
          .SetIsOriginAllowed(origin => true)
          .AllowCredentials();
});

app.UseHttpsRedirection();
app.UseAuthorization();

app.MapControllers();
app.MapHub<ChatHub>("/chatHub");

app.Run();