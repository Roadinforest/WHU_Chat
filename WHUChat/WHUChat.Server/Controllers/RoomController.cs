using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WHUChat.Server.Services;
using WHUChat.Server.DTOs.Room;
using WHUChat.Server.Common; 
using System.Security.Claims;
using System.Threading.Tasks;
using System.Collections.Generic; // For List
using Microsoft.Extensions.Logging; // For logging

namespace WHUChat.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize] // 整个 Controller 都需要授权
    public class RoomController : ControllerBase
    {
        private readonly IRoomService _roomService;
        private readonly ILogger<RoomController> _logger;

        public RoomController(IRoomService roomService, ILogger<RoomController> logger)
        {
            _roomService = roomService;
            _logger = logger;
        }

        private long GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier) ?? User.FindFirst("id");
            if (userIdClaim == null || !long.TryParse(userIdClaim.Value, out var userId))
            {
                throw new UnauthorizedAccessException("无法从 Token 中获取有效的用户 ID。");
            }
            return userId;
        }

        // POST /api/room - 创建房间
        [HttpPost]
        public async Task<ActionResult<Result<RoomResponseDto>>> CreateRoom([FromBody] CreateRoomRequestDto dto)
        {
            try
            {
                long creatorId = GetCurrentUserId();
                var room = await _roomService.CreateRoomAsync(creatorId, dto);
                return Ok(Result<RoomResponseDto>.Ok(room, "房间创建成功"));
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex, "创建房间失败: {ErrorMessage}", ex.Message);
                return NotFound(Result<object>.Fail(ex.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "创建房间时发生意外错误。");
                return StatusCode(500, Result<object>.Fail("创建房间失败：" + ex.Message));
            }
        }

        // POST /api/room/{roomId}/join - 加入房间
        [HttpPost("{roomId}/join")]
        public async Task<ActionResult<Result<object>>> JoinRoom(long roomId)
        {
            try
            {
                long userId = GetCurrentUserId();
                await _roomService.JoinRoomAsync(userId, roomId);
                return Ok(Result<object>.Ok(null, "成功加入房间"));
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex, "加入房间失败 (Room: {RoomId}, User: {UserId}): {ErrorMessage}", roomId, GetCurrentUserId(), ex.Message);
                return NotFound(Result<object>.Fail(ex.Message));
            }
            catch (InvalidOperationException ex) // 如已是成员
            {
                _logger.LogWarning(ex, "加入房间操作无效 (Room: {RoomId}, User: {UserId}): {ErrorMessage}", roomId, GetCurrentUserId(), ex.Message);
                return Conflict(Result<object>.Fail(ex.Message)); // 409 Conflict
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "加入房间时发生意外错误 (Room: {RoomId}, User: {UserId})。", roomId, GetCurrentUserId());
                return StatusCode(500, Result<object>.Fail("加入房间失败：" + ex.Message));
            }
        }

        // DELETE /api/room/{roomId}/leave - 退出房间
        [HttpDelete("{roomId}/leave")]
        public async Task<ActionResult<Result<object>>> LeaveRoom(long roomId)
        {
            try
            {
                long userId = GetCurrentUserId();
                await _roomService.LeaveRoomAsync(userId, roomId);
                return Ok(Result<object>.Ok(null, "成功退出房间"));
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex, "退出房间失败 (Room: {RoomId}, User: {UserId}): {ErrorMessage}", roomId, GetCurrentUserId(), ex.Message);
                return NotFound(Result<object>.Fail(ex.Message));
            }
            catch (InvalidOperationException ex) // 如不是成员
            {
                _logger.LogWarning(ex, "退出房间操作无效 (Room: {RoomId}, User: {UserId}): {ErrorMessage}", roomId, GetCurrentUserId(), ex.Message);
                return BadRequest(Result<object>.Fail(ex.Message)); // 400 Bad Request 
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "退出房间时发生意外错误 (Room: {RoomId}, User: {UserId})。", roomId, GetCurrentUserId());
                return StatusCode(500, Result<object>.Fail("退出房间失败：" + ex.Message));
            }
        }

        // GET /api/room/joined - 查看已加入的房间列表
        [HttpGet("joined")]
        public async Task<ActionResult<Result<List<RoomResponseDto>>>> GetJoinedRooms()
        {
            try
            {
                long userId = GetCurrentUserId();
                var rooms = await _roomService.GetJoinedRoomsAsync(userId);
                return Ok(Result<List<RoomResponseDto>>.Ok(rooms, "获取成功"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "获取已加入房间列表时发生意外错误 (User: {UserId})。", GetCurrentUserId());
                return StatusCode(500, Result<List<RoomResponseDto>>.Fail("获取列表失败：" + ex.Message));
            }
        }

        // GET /api/room/{roomId}/members - 查看指定房间成员列表
        [HttpGet("{roomId}/members")]
        public async Task<ActionResult<Result<List<RoomMemberResponseDto>>>> GetRoomMembers(long roomId)
        {
            try
            {
                long requestingUserId = GetCurrentUserId();
                var members = await _roomService.GetRoomMembersAsync(roomId, requestingUserId);
                return Ok(Result<List<RoomMemberResponseDto>>.Ok(members, "获取成功"));
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex, "获取房间成员失败 (Room: {RoomId}): {ErrorMessage}", roomId, ex.Message);
                return NotFound(Result<object>.Fail(ex.Message));
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogWarning(ex, "无权查看房间成员 (Room: {RoomId}, User: {UserId})。", roomId, GetCurrentUserId());
                return Forbid(); // 403 Forbidden
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "获取房间成员时发生意外错误 (Room: {RoomId})。", roomId);
                return StatusCode(500, Result<List<RoomMemberResponseDto>>.Fail("获取成员失败：" + ex.Message));
            }
        }

        // POST /api/room/{roomId}/invite - 邀请用户加入房间
        [HttpPost("{roomId}/invite")]
        public async Task<ActionResult<Result<object>>> InviteUserToRoom(long roomId, [FromBody] InviteUserToRoomRequestDto dto)
        {
            try
            {
                long inviterId = GetCurrentUserId();
                await _roomService.InviteUserToRoomAsync(inviterId, roomId, dto.InvitedUserId);
                return Ok(Result<object>.Ok(null, "邀请成功，用户已加入房间"));
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex, "邀请用户加入房间失败 (Room: {RoomId}, Inviter: {InviterId}, Invited: {InvitedUserId}): {ErrorMessage}", roomId, GetCurrentUserId(), dto.InvitedUserId, ex.Message);
                return NotFound(Result<object>.Fail(ex.Message));
            }
            catch (InvalidOperationException ex) // 如已是成员
            {
                _logger.LogWarning(ex, "邀请用户加入房间操作无效 (Room: {RoomId}, Inviter: {InviterId}, Invited: {InvitedUserId}): {ErrorMessage}", roomId, GetCurrentUserId(), dto.InvitedUserId, ex.Message);
                return Conflict(Result<object>.Fail(ex.Message));
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogWarning(ex, "无权邀请用户加入房间 (Room: {RoomId}, Inviter: {InviterId})。", roomId, GetCurrentUserId());
                return Forbid();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "邀请用户加入房间时发生意外错误 (Room: {RoomId}, Inviter: {InviterId}, Invited: {InvitedUserId})。", roomId, GetCurrentUserId(), dto.InvitedUserId);
                return StatusCode(500, Result<object>.Fail("邀请失败：" + ex.Message));
            }
        }
    }
}