using Microsoft.AspNetCore.Identity.Data;
using System.IdentityModel.Tokens.Jwt;
using WHUChat.Server.Models;
using WHUChat.Server.Repositories;
using WHUChat.Server.DTOs;
using WHUChat.Server.Utils;
using Microsoft.EntityFrameworkCore;

namespace WHUChat.Server.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _repo;
        public UserService(IUserRepository repo) => _repo = repo;

        public void Register(RegisterRequestDto req)
        {
            if (req.Username == null || req.Password == null) throw new Exception("用户名和密码为必填项");
            var user = new User { 
                Username = req.Username, 
                Password = BCrypt.Net.BCrypt.HashPassword(req.Password),
                AvatarUrl = "https://www.dummyimage.com/600x400/fff/000000&text=" + req.Username[0],
                CreatedAt = DateTime.Now
            };
            _repo.Add(user);
        }

        public string Login(LoginRequestDto req)
        {
            var user = _repo.GetByUsername(req.Username);
            if (user == null || !BCrypt.Net.BCrypt.Verify(req.Password, user.Password)) throw new Exception("用户名或密码错误");
            return JwtHelper.GenerateToken(user);
        }

        public User GetById(long id) => _repo.GetById(id);
    }
}
