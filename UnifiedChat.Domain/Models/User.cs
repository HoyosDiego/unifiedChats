using System.Text.Json.Serialization;
using UnifiedChat.Domain.Models;

public class User
{
    public string userId { get; set; }
    public string userName { get; set; }
    public string email { get; set; }

    [JsonIgnore] // <--- ESTO ROMPE EL BUCLE
    public ICollection<Chat> Chats { get; set; }
}