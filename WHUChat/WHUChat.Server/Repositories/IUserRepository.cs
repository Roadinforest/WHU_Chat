using WHUChat.Server.Models;

namespace WHUChat.Server.Repositories
{
    public interface IUserRepository
    {
        void Add(User user);
        User GetByUsername(string username);
        User GetById(long id);
    }
}
