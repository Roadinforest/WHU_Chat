using System.Diagnostics.Eventing.Reader;
using WHUChat.Server.DTOs.FriendShip;

namespace WHUChat.Server.Services
{
    public interface IFriendService
    {
        Task SendFriendRequestAsync(long senderId, long receiverId);
        Task RespondFriendRequestAsync(long receiverId, long senderId, bool accept);
        Task<List<FriendRequestDto>> GetReceivedRequestsAsync(long userId);
        Task<List<FriendRequestDto>> GetSentRequestsAsync(long userId);
        Task<List<FriendDto>> GetFriendListAsync(long userId);

        Task InsertPrivateRoom(long roomId,long userId1, long userId2); 
        Task DeleteFriendAsync(long userId, long friendId);
    }
}
