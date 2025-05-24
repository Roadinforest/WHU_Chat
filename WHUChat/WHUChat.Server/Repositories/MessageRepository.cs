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
                .ToListAsync();
        }

        public async Task<List<Message>> GetSpecificMessage(long roomId, string username) { 
            return await _context.Messages.Where(m=>m.RoomId==roomId &&m.Username==username)
                .ToListAsync();
        
        }
        public  async Task<long> InsertMessage(Message message) {
            await _context.Messages.AddAsync(message);
            await _context.SaveChangesAsync();
            return message.Id;
        }

        public async Task<long> DeleteMessage(long id) {
            var desti = _context.Messages.Find(id);
            _context.Messages.Remove(desti);
            await _context.SaveChangesAsync();
            return desti.RoomId;

        }

        //public async Task SaveChangesAsync()
        //{
        //    await _context.SaveChangesAsync();
        //}
    }
}
