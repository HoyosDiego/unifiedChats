using UnifiedChat.Domain.Models;

namespace UnifiedChat.Domain.Interfaces
{
    public interface IChatRepository
    {
        Task<IEnumerable<Chat>> GetAllChatsAsync();

        Task<IEnumerable<Chat>> GetChatsByPlatformAsync(string platform);

        Task AddChatAsync(Chat chat);

        Task<bool> SaveChangesAsync();
    }
}