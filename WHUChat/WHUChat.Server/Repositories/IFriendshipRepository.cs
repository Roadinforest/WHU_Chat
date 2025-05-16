using WHUChat.Server.Models;

namespace WHUChat.Server.Repositories
{
    public interface IFriendshipRepository
    {
        Task<FriendRequest?> GetFriendRequestAsync(long senderId, long receiverId);
        Task<bool> DoesRequestExistAsync(long userId1, long userId2);
        Task AddFriendRequestAsync(FriendRequest request);
        Task UpdateFriendRequestAsync(FriendRequest request);
        Task<List<FriendRequest>> GetReceivedPendingRequestsAsync(long userId);
        Task<List<FriendRequest>> GetSentRequestsAsync(long userId);
        Task<bool> AreUsersFriendsAsync(long userId1, long userId2);
        Task AddFriendRelationAsync(FriendRelation relation);
        Task<List<FriendRelation>> GetFriendRelationsAsync(long userId);
        Task<User?> GetUserByIdAsync(long userId); // 可能需要获取用户信息
        Task SaveChangesAsync(); // 暴露 SaveChangesAsync 以便 Service 控制事务
        Task<FriendRelation?> GetFriendRelationAsync(long userId, long friendId); // Helper to find a specific relation
        void DeleteFriendRelation(FriendRelation relation); // Method to mark a relation for deletion
        Task DeleteFriendRelationsAsync(long userId, long friendId); // More direct approach (optional)

        Task DeleteFriendRequestsBetweenAsync(long userId1, long userId2); // 删除两人间的所有请求

        Task CreatePrivateRoom(long roomId,long userId1,long userId2 );
    }
}
