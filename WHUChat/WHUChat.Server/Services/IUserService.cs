using Microsoft.AspNetCore.Identity.Data;
using WHUChat.Server.Models;
using WHUChat.Server.DTOs;

namespace WHUChat.Server.Services
{
    public interface IUserService
    {
        void Register(RegisterRequestDto request);
        string Login(LoginRequestDto request);
        User GetById(int id);
    }
}
