using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WHUChat.Server.Common;
using WHUChat.Server.Services;
using System.Security.Claims;
using WHUChat.Server.DTOs; // 确保引入 DTO

namespace WHUChat.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize] // 确保所有接口都需要认证
    public class UserController : ControllerBase // 继承 ControllerBase 以便使用 Ok(), NotFound() 等 IActionResult 方法
    {
        private readonly IUserService _userService;
        private readonly ILogger<AuthController> _logger;
        public UserController(IUserService userService, ILogger<AuthController> logger)
        {
            _userService = userService;
            _logger = logger;
        }
        // Helper to get current user ID from token claims
        private long GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?? User.FindFirst("id"); // 尝试标准和自定义 claim
            if (userIdClaim == null || !long.TryParse(userIdClaim.Value, out var userId))
            {
                // 如果在 Authorize 后仍然无法获取 ID，则说明 Token 或配置有问题
                throw new UnauthorizedAccessException("无法从 Token 中获取有效的用户 ID。");
            }
            return userId;
        }

        /// <summary>
        /// 获取当前登录用户信息
        /// </summary>
        /// <returns>包含操作结果和当前用户信息的 Result</returns>
        [HttpGet("me")]
        public ActionResult<Result<UserDto>> GetCurrentUser()
        {
            try
            {
                var userId = GetCurrentUserId();
                var user = _userService.GetById(userId);
                if (user == null)
                {
                    return NotFound(Result<UserDto>.Fail("当前用户信息不存在")); // 返回 404 Not Found 和失败信息
                }
                var response = new UserDto
                {
                    Id = user.Id,
                    Username = user.Username,
                    AvatarUrl = user.AvatarUrl,
                };
                return Ok(Result<UserDto>.Ok(response)); // 返回 200 OK 和成功结果
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogError(ex, "获取当前用户信息时发生未授权异常。");
                return Unauthorized(Result<UserDto>.Fail("未授权")); // 返回 401 Unauthorized 和失败信息
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "获取当前用户信息时发生错误。");
                return StatusCode(500, Result<UserDto>.Fail("服务器内部错误")); // 返回 500 Internal Server Error 和失败信息
            }
        }

        /// <summary>
        /// 根据用户 ID 获取用户信息
        /// </summary>
        /// <param name="id">要查询的用户 ID</param>
        /// <returns>包含操作结果和指定用户信息的 Result</returns>
        [HttpGet("{id}")]
        public ActionResult<Result<UserDto>> GetUserById(long id)
        {
            try
            {
                var user = _userService.GetById(id);
                if (user == null)
                {
                    return NotFound(Result<UserDto>.Fail($"用户 ID为 {id} 的信息不存在")); // 返回 404 Not Found 和失败信息
                }   
                var response = new UserDto
                {
                    Id = user.Id,
                    Username = user.Username,
                    AvatarUrl = user.AvatarUrl,
                    // 可以添加其他需要返回的字段
                };
                return Ok(Result<UserDto>.Ok(response)); // 返回 200 OK 和成功结果
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"获取用户 ID为 {id} 的信息时发生错误。");
                return StatusCode(500, Result<UserDto>.Fail("服务器内部错误")); // 返回 500 Internal Server Error 和失败信息
            }
        }
    }
}