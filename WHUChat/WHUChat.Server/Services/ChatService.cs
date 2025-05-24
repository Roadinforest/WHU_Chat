using Microsoft.VisualBasic;
using WHUChat.Server.DTOs;
using WHUChat.Server.Models;
using WHUChat.Server.Repositories;
namespace WHUChat.Server.Services
{
    public class ChatService:IChatService
    {
        private readonly IMessageRepository _messageRepository;
        private readonly ILogger<IChatService> _logger;

        public ChatService(IMessageRepository messageRepository, ILogger<IChatService> logger)
        {
            _messageRepository = messageRepository;
            _logger = logger;
        }

        public async Task<List<SendMessageRequestDto>> GetHistory(long roomId) { 
            List<Message> messages= await _messageRepository.GetHistory(roomId);
            List<SendMessageRequestDto> messagesList= new List<SendMessageRequestDto>();

            foreach (Message m in messages) {
                messagesList.Add(new SendMessageRequestDto
                {
                    Id = m.Id,
                    UserName = m.Username,
                    Content = m.Content,
                    ResUrl = m.Resurl,
                });
            }
            return messagesList;
        
        }

        public async Task<List<SendMessageRequestDto>> GetSpecificMessage(long roomId, string username) {
            List<Message> messages=await _messageRepository.GetSpecificMessage(roomId, username);
            List<SendMessageRequestDto> messageList= new List<SendMessageRequestDto>();

            foreach (Message m in messages) {
                messageList.Add(new SendMessageRequestDto
                {
                    Id = m.Id,
                    UserName = m.Username,
                    Content = m.Content,
                    ResUrl = m.Resurl,

                });
            }
            return messageList;
        }
        public async Task<long> InsertMessage(Message m) { 
            return await _messageRepository.InsertMessage(m);
            
        }

        public async Task<long> DeleteMessage(long id) {
            return await _messageRepository.DeleteMessage(id);
        }

    }
}
