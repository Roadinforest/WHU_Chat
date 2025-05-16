using Microsoft.EntityFrameworkCore;
using WHUChat.Server.Data;
using WHUChat.Server.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WHUChat.Server.Repositories
{
    public class RoomRepository : IRoomRepository
    {
        private readonly AppDbContext _context;

        public RoomRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Room> CreateRoomAsync(Room roomWithInitialMember)
        {
            // 假设 roomWithInitialMember.RoomMembers 集合已经包含了初始成员
            // EF Core 在保存 roomWithInitialMember 时会自动处理其 RoomMembers
            await _context.Rooms.AddAsync(roomWithInitialMember);
            return roomWithInitialMember;
        }

        public async Task<Room?> GetRoomByIdAsync(long roomId)
        {
            return await _context.Rooms
                .Include(r => r.Creator) // 加载创建者信息
                .FirstOrDefaultAsync(r => r.Id == roomId);
        }

        public async Task<bool> IsUserMemberAsync(long roomId, long userId)
        {
            return await _context.RoomMembers.AnyAsync(rm => rm.RoomId == roomId && rm.MemberId == userId);
        }

        public async Task AddMemberAsync(RoomMember member)
        {
            // 检查是否已是成员，避免重复插入 (虽然主键约束会阻止)
            var existing = await _context.RoomMembers
                .FirstOrDefaultAsync(rm => rm.RoomId == member.RoomId && rm.MemberId == member.MemberId);
            if (existing == null)
            {
                await _context.RoomMembers.AddAsync(member);
            }
        }

        public async Task<bool> RemoveMemberAsync(long roomId, long userId)
        {
            var member = await _context.RoomMembers
                .FirstOrDefaultAsync(rm => rm.RoomId == roomId && rm.MemberId == userId);
            if (member != null)
            {
                _context.RoomMembers.Remove(member);
                return true;
            }
            return false;
        }

        public async Task<List<Room>> GetRoomsForUserAsync(long userId)
        {
            // 修改后的查询：直接从 Rooms 开始
            return await _context.Rooms // 1. 查询从 _context.Rooms (IQueryable<Room>) 开始
                .Where(r => r.RoomMembers.Any(rm => rm.MemberId == userId)) // 2. 筛选出那些 RoomMembers 集合中包含指定 userId 的房间
                .Include(r => r.Creator) // 3. 现在 Include 是直接应用在 IQueryable<Room> 上，这通常更可靠
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync();
        }

        public async Task<List<RoomMember>> GetMembersInRoomAsync(long roomId)
        {
            return await _context.RoomMembers
                .Where(rm => rm.RoomId == roomId)
                .Include(rm => rm.Member) // 加载成员的 User 信息
                .ToListAsync();
        }
        public async Task<int> GetRoomMemberCountAsync(long roomId)
        {
            return await _context.RoomMembers.CountAsync(rm => rm.RoomId == roomId);
        }

        public async Task<User?> GetUserByIdAsync(long userId) // 辅助方法
        {
            return await _context.Users.FindAsync(userId);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}