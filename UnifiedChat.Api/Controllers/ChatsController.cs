using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UnifiedChat.Infrastructure.Persistence;
using UnifiedChat.Domain.Models;

namespace UnifiedChat.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ChatsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ChatsController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/chats
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Chat>>> GetChats()
        {
            // Usamos Include para traer también la información del Usuario (el JOIN)
            var chats = await _context.Chats
                .Include(c => c.User)
                .ToListAsync();

            if (chats == null)
            {
                return NotFound();
            }

            return Ok(chats);
        }

        // GET: api/chats/platform/tiktok
        [HttpGet("platform/{platform}")]
        public async Task<ActionResult<IEnumerable<Chat>>> GetChatsByPlatform(string platform)
        {
            var chats = await _context.Chats
                .Where(c => c.typePlatform.ToLower() == platform.ToLower())
                .ToListAsync();

            return Ok(chats);
        }
    }
}