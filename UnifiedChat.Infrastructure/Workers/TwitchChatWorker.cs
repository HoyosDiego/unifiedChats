using System.Text;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using TwitchLib.Client;
using TwitchLib.Client.Models;
using TwitchLib.Communication.Clients;
using TwitchLib.Communication.Models;
using UnifiedChat.Domain.Interfaces;
using UnifiedChat.Domain.Models;

namespace UnifiedChat.Infrastructure.Workers;

public class TwitchChatWorker : BackgroundService
{
    private readonly ILogger<TwitchChatWorker> _logger;
    private readonly IMessageBus _messageBus;
    private TwitchClient? _client;
  

    public TwitchChatWorker(ILogger<TwitchChatWorker> logger, IMessageBus messageBus)
    {
        _logger = logger;
        _messageBus = messageBus;
    }
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Iniciando Worker de Twitch...");

        var credentials = new ConnectionCredentials(nick, token);

        var clientOptions = new ClientOptions
        {
            MessagesAllowedInPeriod = 750,
            ThrottlingPeriod = TimeSpan.FromSeconds(30)
        };

        WebSocketClient customClient = new WebSocketClient(clientOptions);
        _client = new TwitchClient(customClient);

        _client.Initialize(credentials, channel);

        _client.OnMessageReceived += async (s, e) => {
            if (string.IsNullOrEmpty(e.ChatMessage.Message))
            {
                return;
            }

            if (e.ChatMessage == null)
            {
                return;
            }
            string finalContent = ProcessTwitchMessage(e.ChatMessage);

            var newMessage = new StreamMessage(
                Guid.NewGuid().ToString(),
                e.ChatMessage.DisplayName ?? e.ChatMessage.Username,
                finalContent,
                "Twitch",
                DateTime.UtcNow
            );

            _logger.LogInformation($"[Twitch Console] {newMessage.UserName}: {finalContent}");
            _messageBus.Writer.TryWrite(newMessage);
        };

        _client.OnConnected += (s, e) =>
            _logger.LogInformation($"Conectado exitosamente a Twitch como {nick}");

        _client.OnConnectionError += (s, e) =>
            _logger.LogError($"Error de conexión: {e.Error.Message}");

        _client.Connect();

        try
        {
            await Task.Delay(-1, stoppingToken);
        }
        catch (OperationCanceledException)
        {
            _logger.LogWarning("Deteniendo Worker de Twitch...");
        }
        finally
        {
            if (_client.IsConnected) _client.Disconnect();
        }
    }

    private string ProcessTwitchMessage(ChatMessage chatMessage)
    {
        var messageText = chatMessage.Message;

        if (chatMessage.EmoteSet == null || !chatMessage.EmoteSet.Emotes.Any())
        {
            return messageText;
        }

        var sortedEmotes = chatMessage.EmoteSet.Emotes
            .OrderByDescending(e => e.StartIndex)
            .ToList();

        var sb = new StringBuilder(messageText);

        foreach (var emote in sortedEmotes)
        {
            string emoteUrl = $"https://static-cdn.jtvnw.net/emoticons/v2/{emote.Id}/static/light/3.0";

            int lengthToRemove = emote.EndIndex - emote.StartIndex + 1;

            sb.Remove(emote.StartIndex, lengthToRemove);
            sb.Insert(emote.StartIndex, emoteUrl);
        }

        return sb.ToString();
    }
}