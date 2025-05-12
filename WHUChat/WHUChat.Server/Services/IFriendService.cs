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
        Task DeleteFriendAsync(long userId, long friendId);
    }
}
