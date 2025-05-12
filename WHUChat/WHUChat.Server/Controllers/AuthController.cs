using Microsoft.AspNetCore.Mvc;
using WHUChat.Server.Services;
using WHUChat.Server.DTOs;
using WHUChat.Server.Common;


namespace WHUChat.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IUserService _userService;
        public AuthController(IUserService userService) => _userService = userService;

        [HttpPost("register")]
        public Result<object> Register(RegisterRequestDto request)
        {
            _userService.Register(request);
            return Result<object>.Ok(null, "注册成功");
        }

        [HttpPost("login")]
        public Result<string> Login(LoginRequestDto request)
        {
            var token = _userService.Login(request);
            return Result<string>.Ok(token, "登陆成功");
        }
    }
}
