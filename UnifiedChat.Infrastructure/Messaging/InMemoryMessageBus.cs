using System.Threading.Channels;
using UnifiedChat.Domain.Interfaces;
using UnifiedChat.Domain.Models;

namespace UnifiedChat.Infrastructure.Messaging;
    public class InMemoryMessageBus : IMessageBus
    {
        private readonly Channel<StreamMessage> _channel;

        public InMemoryMessageBus()
        {
            _channel = Channel.CreateUnbounded<StreamMessage>(new UnboundedChannelOptions
            {
                SingleReader = true,
                SingleWriter = true
            });
        }

        public ChannelWriter<StreamMessage> Writer => _channel.Writer;
        public ChannelReader<StreamMessage> Reader => _channel.Reader;
    }

