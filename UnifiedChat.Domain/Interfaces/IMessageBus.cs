using System.Threading.Channels;
using UnifiedChat.Domain.Models;

namespace UnifiedChat.Domain.Interfaces
{
    public interface IMessageBus
    {
        ChannelWriter<StreamMessage> Writer { get; }
        ChannelReader<StreamMessage> Reader { get; }
    }
}
