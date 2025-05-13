using WHUChat.Server.DTOs.Room;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace WHUChat.Server.Services
{
    public interface IRoomService
    {
        Task<RoomResponseDto> CreateRoomAsync(long creatorId, CreateRoomRequestDto dto);
        Task JoinRoomAsync(long userId, long roomId);
        Task LeaveRoomAsync(long userId, long roomId);
        Task<List<RoomResponseDto>> GetJoinedRoomsAsync(long userId);
        Task<List<RoomMemberResponseDto>> GetRoomMembersAsync(long roomId, long requestingUserId);
        Task InviteUserToRoomAsync(long inviterId, long roomId, long invitedUserId);
    }
}