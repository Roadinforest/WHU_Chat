using Microsoft.AspNetCore.Mvc;
using WHUChat.Server.Services;
using WHUChat.Server.DTOs;
using WHUChat.Server.Common;
using WHUChat.Server.Models;


namespace WHUChat.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IChatService _chatService;
        private readonly ILogger<AuthController> _logger;
        public AuthController(IUserService userService, ILogger<AuthController> logger, IChatService chatService)
        {
            _userService = userService;
            _logger = logger;
            _chatService = chatService;
        }

        [HttpPost("register")]
        public Result<object> Register(RegisterRequestDto request)
        {
            try
            {
                _userService.Register(request);
            }
            catch (Exception ex)
            {
                _logger.LogInformation(ex.Message);
                return Result<object>.Fail("注册失败");
            }
            return Result<object>.Ok(null, "注册成功");
        }

        [HttpPost("login")]
        public Result<string> Login(LoginRequestDto request)
        {
            string token;
            try
            {
                token = _userService.Login(request);
            }
            catch (Exception ex)
            {
                _logger.LogInformation(ex.Message);
                return Result<string>.Fail("登录失败");
            }
            return Result<string>.Ok(token, "登陆成功");
        }
    }
}
