namespace UnifiedChat.Domain.Models
{
    public class Chat
    {
        public string userId { get; set; } // PK que creamos
        public string comment { get; set; }
        public string typePlatform { get; set; }
        public DateTime dateChats { get; set; }
        public User User { get; set; } // Propiedad de navegación
    }
}