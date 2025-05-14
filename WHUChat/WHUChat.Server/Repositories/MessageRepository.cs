using Microsoft.EntityFrameworkCore;
using WHUChat.Server.Data;
using WHUChat.Server.Models;

namespace WHUChat.Server.Repositories
{
    public class MessageRepository:IMessageRepository
    {
        private readonly AppDbContext _context;
        public MessageRepository(AppDbContext context) => _context = context;

        public async Task<List<Message>> GetHistory(long roomId) {
            return await _context.Messages.Where(m => m.RoomId == roomId)
                .Include(m => m.Content)
                .ToListAsync();
        }

        public async Task<List<Message>> GetSpecificMessage(long roomId, string username) { 
            return await _context.Messages.Where(m=>m.RoomId==roomId &&m.UserName==username)
                .Include(m => m.Content)
                .ToListAsync();
        
        }
        public async Task InsertMessage(Message message) {
            await _context.Messages.AddAsync(message);
            await _context.SaveChangesAsync();
        }
    }
}
