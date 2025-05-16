using WHUChat.Server.DTOs;
using WHUChat.Server.Models;

namespace WHUChat.Server.Services
{
    public interface IChatService
    {
        public Task<List<SendMessageRequestDto>> GetHistory(long roomId);

        public Task<List<SendMessageRequestDto>> GetSpecificMessage(long roomId, string username);

        public Task InsertMessage(Message messasge);
    }
}
