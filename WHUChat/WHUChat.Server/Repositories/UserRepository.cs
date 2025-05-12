using Microsoft.EntityFrameworkCore;
using WHUChat.Server.Models;
using WHUChat.Server.Data;

namespace WHUChat.Server.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly AppDbContext _context;
        public UserRepository(AppDbContext context) => _context = context;

        public void Add(User user) { _context.Users.Add(user); _context.SaveChanges(); }

        public User GetByUsername(string username) => _context.Users.FirstOrDefault(u => u.Username == username);

        public User GetById(int id) => _context.Users.Find(id);
    }
}
