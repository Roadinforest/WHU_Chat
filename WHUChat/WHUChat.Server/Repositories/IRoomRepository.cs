using WHUChat.Server.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace WHUChat.Server.Repositories
{
    public interface IRoomRepository
    {
        Task<Room> CreateRoomAsync(Room room);
        Task<Room?> GetRoomByIdAsync(long roomId);
        Task<bool> IsUserMemberAsync(long roomId, long userId);
        Task AddMemberAsync(RoomMember member);
        Task<bool> RemoveMemberAsync(long roomId, long userId);
        Task<List<Room>> GetRoomsForUserAsync(long userId); // 获取用户加入的所有房间
        Task<List<RoomMember>> GetMembersInRoomAsync(long roomId); // 获取房间的所有成员关系
        Task<User?> GetUserByIdAsync(long userId); // 通常可能在 IUserRepository
        Task<int> GetRoomMemberCountAsync(long roomId);

        Task SaveChangesAsync();
    }
}