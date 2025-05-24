using Microsoft.EntityFrameworkCore;
using WHUChat.Server.DTOs;
using WHUChat.Server.Models;

namespace WHUChat.Server.Repositories
{
    public interface IMessageRepository
    {
        public Task<List<Message>> GetHistory(long roomId);

        public Task<List<Message>> GetSpecificMessage(long roomId, string username);
        public Task<long> InsertMessage(Message message);

        public Task DeleteMessage(long id);

        //public Task SaveChangesAsync();
        
    }
}
