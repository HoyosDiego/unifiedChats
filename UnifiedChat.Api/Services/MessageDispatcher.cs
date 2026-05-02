using Microsoft.AspNetCore.SignalR;
using UnifiedChat.Api.Hubs;
using UnifiedChat.Domain.Interfaces;

namespace UnifiedChat.Api.Services;

public class MessageDispatcher : BackgroundService
{
    private readonly IMessageBus _messageBus;
    private readonly IHubContext<ChatHub> _hubContext;
    private readonly ILogger<MessageDispatcher> _logger;

    public MessageDispatcher(
        IMessageBus messageBus,
        IHubContext<ChatHub> hubContext,
        ILogger<MessageDispatcher> logger)
    {
        _messageBus = messageBus;
        _hubContext = hubContext;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                var message = await _messageBus.Reader.ReadAsync(stoppingToken);

                await _hubContext.Clients.All.SendAsync(
                    "ReceiveMessage",
                    message,
                    cancellationToken: stoppingToken
                );

                _logger.LogInformation($"Despachado: [{message.Platform}] {message.UserName}");
            }
            catch (OperationCanceledException)
            {
                break;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error en Dispatcher: {ex.Message}");
            }
        }
    }
}