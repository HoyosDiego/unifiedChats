using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using TikTokLiveSharp.Client;
using UnifiedChat.Domain.Interfaces;
using UnifiedChat.Domain.Models;

namespace UnifiedChat.Infrastructure.Workers;
public class TikTokChatWorker : BackgroundService
{
    private readonly ILogger<TikTokChatWorker> _logger;
    private readonly IMessageBus _messageBus;
    private TikTokLiveClient? _client;

    public TikTokChatWorker(ILogger<TikTokChatWorker> logger, IMessageBus messageBus)
    {
        _logger = logger;
        _messageBus = messageBus;
    }
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await Task.Delay(5000, stoppingToken);

            try
            {
                _logger.LogInformation($"[TikTok] Intentando conectar con el usuario: {tikTokUser}");

                _client = new TikTokLiveClient(uniqueID: tikTokUser);

                _client.OnRoomUpdate += (sender, e) =>
                {
                    var conteo = e.NumberOfViewers;
                    _logger.LogInformation($"[TikTok] Personas en vivo: {conteo}");
                };

                _client.OnChatMessage += (sender, args) =>
                {
                    if (args == null || args.Sender == null || string.IsNullOrEmpty(args.Message))
                    {
                        return;
                    }

                    var newMessage = new StreamMessage(
                        Guid.NewGuid().ToString(),
                        args.Sender.UniqueId ?? "TikTokUser",
                        args.Message,
                        "TikTok",
                        DateTime.UtcNow
                    );

                    _logger.LogInformation($"[TikTok Console] {newMessage.UserName}: {newMessage.Message}");

                    _messageBus.Writer.TryWrite(newMessage);
                };

                await Task.Run(() =>
                {
                    try
                    {
                        _client.Run();
                    }
                    catch (InvalidOperationException)
                    {
                        _logger.LogWarning($"[TikTok] @{tikTokUser} parece estar fuera de línea.");
                    }
                }, stoppingToken);

            }
            catch (Exception ex)
            {
                _logger.LogError($"[TikTok] Error de conexión: {ex.Message}");
            }

            _logger.LogInformation("[TikTok] Reintentando conexión en 60 segundos...");
            await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
        }
}