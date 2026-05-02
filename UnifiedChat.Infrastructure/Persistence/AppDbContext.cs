using Microsoft.EntityFrameworkCore;
using UnifiedChat.Domain.Models;

namespace UnifiedChat.Infrastructure.Persistence
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Chat> Chats { get; set; }
        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Configuración para la tabla Chats
            modelBuilder.Entity<Chat>(entity =>
            {
                entity.ToTable("Chats");
                entity.HasKey(c => c.userId); // Asegúrate que en tu clase Chat se escriba así (uId minúscula)

                // EXPLICAR LA RELACIÓN PARA EVITAR EL 'userId1'
                entity.HasOne(c => c.User)      // Un Chat tiene un Usuario
                    .WithMany(u => u.Chats)     // Un Usuario tiene muchos Chats
                    .HasForeignKey(c => c.userId); // La llave foránea es EXACTAMENTE 'userId'
            });

            // Configuración para la tabla Users
            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable("Users");
                entity.HasKey(u => u.userId);
            });
        }
    }
}