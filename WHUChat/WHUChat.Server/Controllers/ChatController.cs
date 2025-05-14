using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WHUChat.Server.Services;
using WHUChat.Server.Common;
using System.Security.Claims;
using WHUChat.Server.DTOs; // 用于获取 User ID


namespace WHUChat.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize] // 确保所有接口都需要认证
    public class ChatController : ControllerBase
    {
        private readonly IChatService _chatService;
        private readonly ILogger<ChatController> _logger;

        public ChatController(IChatService chatService, ILogger<ChatController> logger) {
            _chatService = chatService;
            _logger = logger;
        }

        // Helper to get current user ID from token claims
        //private long GetCurrentUserId()
        //{
        //    var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier) ?? User.FindFirst("id");
        //    if (userIdClaim == null || !long.TryParse(userIdClaim.Value, out var userId))
        //    {
        //        // 如果在 Authorize 后仍然无法获取 ID，则说明 Token 或配置有问题
        //        throw new UnauthorizedAccessException("无法从 Token 中获取有效的用户 ID。");
        //    }
        //    return userId;
        //}


        //在EntryRoom(signalR)前调用，获取历史记录
        [HttpGet("{roomId}/get_history")]
        public async Task<ActionResult<Result<object>>> GetHistory(long roomId)
        {
            try {
                var messages = await _chatService.GetHistory(roomId);
                return Ok(Result<List<SendMessageRequestDto>>.Ok(messages, "获取成功"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "获取房间历史消息时发生意外错误 (Room: {RoomId})。", roomId);
                return StatusCode(500, Result<List<SendMessageRequestDto>>.Fail("获取历史消息失败：" + ex.Message));
            }
        }

        //查找特定用户的信息
        [HttpGet("{roomId}/get_specific_message")]
        public async Task<ActionResult<Result<object>>> GetSpecificMessage(long roomId, string username) {
            try
            {
                var messages = await _chatService.GetSpecificMessage(roomId,username);
                return Ok(Result<List<SendMessageRequestDto>>.Ok(messages, "获取成功"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "获取目标用户消息时发生意外错误 (Room: {RoomId},User: {UserName})。", roomId,username);
                return StatusCode(500, Result<List<SendMessageRequestDto>>.Fail("目标用户消息不存在"));
            }
        }
    }
}
