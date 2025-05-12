using Microsoft.EntityFrameworkCore;
using WHUChat.Server.Models;

namespace WHUChat.Server.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options) { }
        public DbSet<User> Users => Set<User>();
        public DbSet<FriendRequest> FriendRequests => Set<FriendRequest>();
        public DbSet<FriendRelation> FriendRelations => Set<FriendRelation>();
        public DbSet<Room> Rooms => Set<Room>();
        public DbSet<RoomMember> RoomMembers => Set<RoomMember>();
        public DbSet<Message> Messages => Set<Message>();
    }
}
