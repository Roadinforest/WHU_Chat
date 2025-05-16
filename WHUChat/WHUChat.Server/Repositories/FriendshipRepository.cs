using WHUChat.Server.Data;
using WHUChat.Server.Models;
using Microsoft.EntityFrameworkCore;

namespace WHUChat.Server.Repositories
{
    public class FriendshipRepository : IFriendshipRepository
    {
        private readonly AppDbContext _context;

        public FriendshipRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<FriendRequest?> GetFriendRequestAsync(long senderId, long receiverId)
        {
            return await _context.FriendRequests
                .FirstOrDefaultAsync(fr => fr.SenderId == senderId && fr.ReceiverId == receiverId);
        }

        public async Task<bool> DoesRequestExistAsync(long userId1, long userId2)
        {
            // 检查两个方向是否存在请求
            return await _context.FriendRequests.AnyAsync(fr =>
                (fr.SenderId == userId1 && fr.ReceiverId == userId2) ||
                (fr.SenderId == userId2 && fr.ReceiverId == userId1));
        }


        public async Task AddFriendRequestAsync(FriendRequest request)
        {
            await _context.FriendRequests.AddAsync(request);
        }

        public Task UpdateFriendRequestAsync(FriendRequest request)
        {
            _context.FriendRequests.Update(request);
            // 不需要 async/await，因为 Update 只是标记实体为 Modified 状态
            return Task.CompletedTask;
        }

        public async Task<List<FriendRequest>> GetReceivedPendingRequestsAsync(long userId)
        {
            return await _context.FriendRequests
                .Include(fr => fr.Sender) // 加载发送者信息
                .Where(fr => fr.ReceiverId == userId && fr.Status == FriendRequestStatus.PENDING)
                .OrderByDescending(fr => fr.CreatedAt)
                .ToListAsync();
        }

        public async Task<List<FriendRequest>> GetSentRequestsAsync(long userId)
        {
            return await _context.FriendRequests
                .Include(fr => fr.Receiver) // 加载接收者信息
                .Where(fr => fr.SenderId == userId)
                .OrderByDescending(fr => fr.CreatedAt)
                .ToListAsync();
        }

        public async Task<bool> AreUsersFriendsAsync(long userId1, long userId2)
        {
            // 检查 friend_relations 表中是否存在关系
            return await _context.FriendRelations
                .AnyAsync(r => (r.UserId == userId1 && r.FriendId == userId2) || (r.UserId == userId2 && r.FriendId == userId1));
        }

        public async Task AddFriendRelationAsync(FriendRelation relation)
        {
            // 检查是否已存在，避免重复插入 (虽然主键约束会阻止，但提前检查更好)
            var exists = await _context.FriendRelations
                                .AnyAsync(fr => fr.UserId == relation.UserId && fr.FriendId == relation.FriendId);
            if (!exists)
            {
                await _context.FriendRelations.AddAsync(relation);
            }
        }

        public async Task<List<FriendRelation>> GetFriendRelationsAsync(long userId)
        {
            // 获取该用户的所有好友关系记录，并加载好友的用户信息
            return await _context.FriendRelations
                .Where(r => r.UserId == userId)
                .Include(r => r.Friend) // 加载关联的好友 User 对象
                .ToListAsync();
        }

        public async Task<User?> GetUserByIdAsync(long userId)
        {
            return await _context.Users.FindAsync(userId);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }

        public async Task<FriendRelation?> GetFriendRelationAsync(long userId, long friendId)
        {
            return await _context.FriendRelations
                .FirstOrDefaultAsync(fr => fr.UserId == userId && fr.FriendId == friendId);
        }

        // Option 1: Method to just mark for deletion (requires service to fetch first)
        public void DeleteFriendRelation(FriendRelation relation)
        {
            _context.FriendRelations.Remove(relation);
        }


        //// Option 2: Direct deletion logic (Preferred for this case)
        //public async Task<bool> DeleteFriendRelationsAsync(long userId, long friendId)
        //{
        //    // Find both relationship entries
        //    var relation1 = await _context.FriendRelations
        //        .FirstOrDefaultAsync(fr => fr.UserId == userId && fr.FriendId == friendId);
        //    var relation2 = await _context.FriendRelations
        //        .FirstOrDefaultAsync(fr => fr.UserId == friendId && fr.FriendId == userId);

        //    bool deleted = false;
        //    if (relation1 != null)
        //    {
        //        _context.FriendRelations.Remove(relation1);
        //        deleted = true; // Mark that at least one relation was found
        //    }
        //    if (relation2 != null)
        //    {
        //        _context.FriendRelations.Remove(relation2);
        //        deleted = true; // Mark that at least one relation was found
        //    }

        //    // We don't call SaveChangesAsync here; the service layer will do that.
        //    return deleted; // Return true if any relation was found and marked for deletion
        //}

        public async Task DeleteFriendRelationsAsync(long userId, long friendId)
        {
            // Find both relationship entries
            var relationsToDelete = await _context.FriendRelations
                .Where(fr => (fr.UserId == userId && fr.FriendId == friendId) || (fr.UserId == friendId && fr.FriendId == userId))
                .ToListAsync(); // 获取所有匹配的记录


            if (relationsToDelete.Any()) // 检查是否有记录需要删除
            {
                _context.FriendRelations.RemoveRange(relationsToDelete); // 使用 RemoveRange 更高效
                                                                         // return true; // 如果需要返回 bool，可以在这里处理
            }
            // return false; // 如果需要返回 bool
            // 或者让 Service 判断 List 是否为空
        }

        public async Task DeleteFriendRequestsBetweenAsync(long userId1, long userId2)
        {
            // 查找 user1 发给 user2 或 user2 发给 user1 的所有好友请求
            var requestsToDelete = await _context.FriendRequests
                .Where(fr => (fr.SenderId == userId1 && fr.ReceiverId == userId2) ||
                             (fr.SenderId == userId2 && fr.ReceiverId == userId1))
                .ToListAsync();

            if (requestsToDelete.Any()) // 如果找到了任何请求
            {
                _context.FriendRequests.RemoveRange(requestsToDelete); // 批量标记为删除
            }
            // 不需要调用 SaveChangesAsync()，由 Service 层统一处理
        }

        public async Task CreatePrivateRoom(long roomId,long userId1,long userId2) {
            await _context.FriendRelations.Where(fr => fr.UserId == userId1&&fr.FriendId==userId2).ExecuteUpdateAsync(s => s.SetProperty(fr => fr.RoomId, roomId));
            await _context.FriendRelations.Where(fr => fr.UserId == userId2&&fr.FriendId==userId1).ExecuteUpdateAsync(s => s.SetProperty(fr => fr.RoomId, roomId));


        }
    }
}

