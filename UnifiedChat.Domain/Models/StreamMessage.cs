
namespace UnifiedChat.Domain.Models;
public record StreamMessage
    (
        String Id,
        String UserName,
        String Message,
        String Platform,
        DateTime CreatedAt
    );

