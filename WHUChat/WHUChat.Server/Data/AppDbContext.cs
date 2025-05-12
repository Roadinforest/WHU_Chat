using Microsoft.EntityFrameworkCore;
using WHUChat.Server.Models;

namespace WHUChat.Server.Data
{
    public class AppDbContext : DbContext
    {

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<FriendRelation>()
                .HasKey(fr => new { fr.UserId, fr.FriendId });

            modelBuilder.Entity<FriendRelation>()
                .HasOne(fr => fr.User)
                .WithMany(u => u.FriendsAdded)
                .HasForeignKey(fr => fr.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<FriendRelation>()
                .HasOne(fr => fr.Friend)
                .WithMany(u => u.FriendsWhoAddedMe)
                .HasForeignKey(fr => fr.FriendId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<FriendRequest>()
                .HasOne(fr => fr.Sender)
                .WithMany(u => u.SentRequests)
                .HasForeignKey(fr => fr.SenderId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<FriendRequest>()
                .HasOne(fr => fr.Receiver)
                .WithMany(u => u.ReceivedRequests)
                .HasForeignKey(fr => fr.ReceiverId)
                .OnDelete(DeleteBehavior.Restrict);

        }

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
