using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WHUChat.Server.Services;
using WHUChat.Server.DTOs.FriendShip;
using WHUChat.Server.DTOs.Room;
using WHUChat.Server.Common;
using System.Security.Claims;
using System.Data.SqlTypes; // 用于获取 User ID

namespace WHUChat.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize] // 确保所有接口都需要认证
    public class FriendController : ControllerBase
    {
        private readonly IFriendService _friendService;
        private readonly IRoomService _roomService;
        private readonly ILogger<FriendController> _logger;

        public FriendController(IFriendService friendService,IRoomService roomService, ILogger<FriendController> logger)
        {
            _friendService = friendService;
            _roomService = roomService;
            _logger = logger;
        }

        private long GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier) ?? User.FindFirst("id"); // 尝试标准和自定义 claim
            if (userIdClaim == null || !long.TryParse(userIdClaim.Value, out var userId))
            {
                // 如果在 Authorize 后仍然无法获取 ID，则说明 Token 或配置有问题
                throw new UnauthorizedAccessException("无法从 Token 中获取有效的用户 ID。");
            }
            return userId;
        }

        // 发送好友申请
        // POST /api/friend/send-request
        [HttpPost("send-request")]
        public async Task<ActionResult<Result<object>>> SendFriendRequest([FromBody] SendFriendRequestDto dto) // 使用 [FromBody]
        {
            try
            {
                long senderId = GetCurrentUserId();
                await _friendService.SendFriendRequestAsync(senderId, dto.ReceiverId);
                return Ok(Result<object>.Ok(null, "申请发送成功")); // 使用标准 Ok() 返回 200
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex, "发送好友申请失败：用户不存在。 Sender: {SenderId}, Receiver: {ReceiverId}", GetCurrentUserId(), dto.ReceiverId);
                return NotFound(Result<object>.Fail(ex.Message)); // 404 Not Found
            }
            catch (InvalidOperationException ex) // 如已经是好友或请求已存在
            {
                _logger.LogWarning(ex, "发送好友申请操作无效。Sender: {SenderId}, Receiver: {ReceiverId}", GetCurrentUserId(), dto.ReceiverId);
                return Conflict(Result<object>.Fail(ex.Message)); // 409 Conflict
            }
            catch (ArgumentException ex) // 如发送给自己
            {
                _logger.LogWarning(ex, "发送好友申请参数错误。Sender: {SenderId}, Receiver: {ReceiverId}", GetCurrentUserId(), dto.ReceiverId);
                return BadRequest(Result<object>.Fail(ex.Message)); // 400 Bad Request
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "发送好友申请时发生意外错误。Sender: {SenderId}, Receiver: {ReceiverId}", GetCurrentUserId(), dto.ReceiverId);
                return StatusCode(500, Result<object>.Fail("发送失败：" + ex.Message)); // 500 Internal Server Error
            }
        }

        // 响应好友申请（接受或拒绝）
        // POST /api/friend/respond-request
        [HttpPost("respond-request")]
        public async Task<ActionResult<Result<object>>> RespondFriendRequest([FromBody] RespondFriendRequestDto dto) // 使用 [FromBody]
        {
            try
            {
                long receiverId = GetCurrentUserId(); // 响应者是当前用户
                await _friendService.RespondFriendRequestAsync(receiverId, dto.SenderId, dto.Accept);
                if (dto.Accept) {//建立好友关系时创建私人对话（抽象为两个成员的room）
                    try
                    {
                        var room = await _roomService.CreateRoomAsync(dto.SenderId, new CreateRoomRequestDto { Name = $"Privateroom_{dto.SenderId.ToString()}_with_{receiverId.ToString()}" });
                        await _roomService.JoinRoomAsync(receiverId, room.Id);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "创建私人对话失败 (User1: {UserId1}, User2: {UserId2})。", dto.SenderId, receiverId);
                        return StatusCode(500, Result<object>.Fail("创建私人对话失败：" + ex.Message));
                    }
                }
                var message = dto.Accept ? "已接受好友申请" : "已拒绝好友申请";
                return Ok(Result<object>.Ok(null, message));
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex, "响应好友申请失败：请求不存在。 Receiver: {ReceiverId}, Sender: {SenderId}", GetCurrentUserId(), dto.SenderId);
                return NotFound(Result<object>.Fail(ex.Message));
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogWarning(ex, "无权响应好友申请。Receiver: {ReceiverId}, Sender: {SenderId}", GetCurrentUserId(), dto.SenderId);
                return Forbid(); // 403 Forbidden
            }
            catch (InvalidOperationException ex) // 如请求已被处理
            {
                _logger.LogWarning(ex, "响应好友申请操作无效。Receiver: {ReceiverId}, Sender: {SenderId}", GetCurrentUserId(), dto.SenderId);
                return Conflict(Result<object>.Fail(ex.Message));
            }
            catch (SqlAlreadyFilledException ex)
            {
                _logger.LogWarning(ex, "响应好友申请失败：双方已经是好友。Receiver: {ReceiverId}, Sender: {SenderId}", GetCurrentUserId(), dto.SenderId);
                return Conflict(Result<object>.Fail(ex.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "响应好友申请时发生意外错误。Receiver: {ReceiverId}, Sender: {SenderId}", GetCurrentUserId(), dto.SenderId);
                return StatusCode(500, Result<object>.Fail("响应失败：" + ex.Message));
            }
        }

        // 查看收到的好友申请 (仅限 PENDING 状态)
        // GET /api/friend/received-requests
        [HttpGet("received-requests")]
        public async Task<ActionResult<Result<List<FriendRequestDto>>>> GetReceivedRequests()
        {
            try
            {
                long userId = GetCurrentUserId();
                var list = await _friendService.GetReceivedRequestsAsync(userId);
                return Ok(Result<List<FriendRequestDto>>.Ok(list, "获取成功"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "获取收到的好友申请失败。 User: {UserId}", GetCurrentUserId());
                // 注意：这里不应返回 List<FriendRequestDto> 的 Fail，Result 结构可能需要调整
                // 返回一个空的成功列表或者一个通用的错误 Result
                return StatusCode(500, Result<List<FriendRequestDto>>.Fail("获取失败：" + ex.Message));
                // 或者: return Ok(Result<List<FriendRequestDto>>.Fail("获取失败：" + ex.Message)); // 如果 Result 支持这种结构
            }
        }

        // 查看已发送的好友申请
        // GET /api/friend/sent-requests
        [HttpGet("sent-requests")]
        public async Task<ActionResult<Result<List<FriendRequestDto>>>> GetSentRequests()
        {
            try
            {
                long userId = GetCurrentUserId();
                var list = await _friendService.GetSentRequestsAsync(userId);
                return Ok(Result<List<FriendRequestDto>>.Ok(list, "获取成功"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "获取发送的好友申请失败。 User: {UserId}", GetCurrentUserId());
                return StatusCode(500, Result<List<FriendRequestDto>>.Fail("获取失败：" + ex.Message));
            }
        }

        // 查看好友列表
        // GET /api/friend/list
        [HttpGet("list")]
        public async Task<ActionResult<Result<List<FriendDto>>>> GetFriendList() // 修改后签名
        {
            try
            {
                long userId = GetCurrentUserId(); // 获取当前登录用户的 ID
                var friends = await _friendService.GetFriendListAsync(userId);
                return Ok(Result<List<FriendDto>>.Ok(friends, "好友列表获取成功")); // 返回具体的 FriendDto 列表
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "获取好友列表失败。 User: {UserId}", GetCurrentUserId());
                // 使用 FriendDto 类型的 Result 返回错误
                return StatusCode(500, Result<List<FriendDto>>.Fail("获取好友列表失败: " + ex.Message));
            }
        }

        // 删除好友
        // DELETE /api/friend/{friendId}
        [HttpDelete("{friendId}")]
        public async Task<ActionResult<Result<object>>> DeleteFriend(long friendId)
        {
            try
            {
                long userId = GetCurrentUserId();
                await _friendService.DeleteFriendAsync(userId, friendId);
                return Ok(Result<object>.Ok(null, "好友删除成功"));
            }
            catch (ArgumentException ex) // e.g., deleting self
            {
                _logger.LogWarning("删除好友参数错误 (User: {UserId}, FriendId: {FriendId}): {ErrorMessage}", GetCurrentUserId(), friendId, ex.Message);
                return BadRequest(Result<object>.Fail(ex.Message)); // 400 Bad Request
            }
            catch (KeyNotFoundException ex) // e.g., friend user doesn't exist
            {
                _logger.LogWarning("删除好友失败，用户不存在 (User: {UserId}, FriendId: {FriendId}): {ErrorMessage}", GetCurrentUserId(), friendId, ex.Message);
                return NotFound(Result<object>.Fail(ex.Message)); // 404 Not Found
            }
            catch (InvalidOperationException ex) // e.g., they were not friends
            {
                _logger.LogWarning("删除好友操作无效 (User: {UserId}, FriendId: {FriendId}): {ErrorMessage}", GetCurrentUserId(), friendId, ex.Message);
                // Could be NotFound (404) or Conflict (409) depending on semantics preference
                return NotFound(Result<object>.Fail(ex.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "删除好友时发生意外错误。 User: {UserId}, FriendId: {FriendId}", GetCurrentUserId(), friendId);
                return StatusCode(500, Result<object>.Fail("删除好友失败：" + ex.Message)); // 500 Internal Server Error
            }
        }
    }
}

