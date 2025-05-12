using Microsoft.EntityFrameworkCore;
using WHUChat.Server.Models;

namespace WHUChat.Server.Data
{
    public class AppDbContext : DbContext
    {

        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<FriendRequest> FriendRequests { get; set; }
        public DbSet<FriendRelation> FriendRelations { get; set; }
        public DbSet<Room> Rooms { get; set; }
        public DbSet<RoomMember> RoomMembers { get; set; }
        public DbSet<Message> Messages { get; set; }
        public DbSet<RoomMessage> RoomMessages { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // 设置表名
            modelBuilder.Entity<User>().ToTable("users");
            modelBuilder.Entity<FriendRelation>().ToTable("friend_relations");
            modelBuilder.Entity<FriendRequest>().ToTable("friend_requests");
            modelBuilder.Entity<RoomMember>().ToTable("room_members");
            modelBuilder.Entity<Room>().ToTable("rooms");

            // 全局配置：将所有属性名转换为小写字母加下划线
            foreach (var entity in modelBuilder.Model.GetEntityTypes())
            {
                foreach (var property in entity.GetProperties())
                {
                    // 处理属性名（PascalCase -> snake_case）
                    property.SetColumnName(ToSnakeCase(property.Name));
                }
            }


                // 配置 FriendRequest 之间的关系（用户发送和接收的好友请求）
                modelBuilder.Entity<FriendRequest>()
                .HasOne(fr => fr.Sender)  // 发送方（用户）
                .WithMany(u => u.SentFriendRequests)  // 一个用户可以有多个发送的好友请求
                .HasForeignKey(fr => fr.SenderId)  // 通过 SenderId 进行关联
                .OnDelete(DeleteBehavior.Cascade);  // 发送方删除时，好友请求级联删除

            modelBuilder.Entity<FriendRequest>()
                .HasOne(fr => fr.Receiver)  // 接收方（用户）
                .WithMany(u => u.ReceivedFriendRequests)  // 一个用户可以有多个接收的好友请求
                .HasForeignKey(fr => fr.ReceiverId)  // 通过 ReceiverId 进行关联
                .OnDelete(DeleteBehavior.Cascade);  // 接收方删除时，好友请求级联删除

            // 配置 FriendRelation 之间的双向关系（表示用户与其好友之间的关系）
            modelBuilder.Entity<FriendRelation>()
                .HasKey(fr => new { fr.UserId, fr.FriendId });  // 复合主键：用户ID + 好友ID

            // 配置 User 与 FriendRelation 的关系
            modelBuilder.Entity<FriendRelation>()
                .HasOne(fr => fr.User)  // 每个 FriendRelation 都有一个关联的用户
                .WithMany(u => u.FriendRelations)  // 一个用户可以有多个 FriendRelation（对应多个好友关系）
                .HasForeignKey(fr => fr.UserId)  // 通过 UserId 进行关联
                .OnDelete(DeleteBehavior.Cascade);  // 用户删除时，相关好友关系级联删除

            // 配置 Friend 与 FriendRelation 的关系
            modelBuilder.Entity<FriendRelation>()
                .HasOne(fr => fr.Friend)  // 每个 FriendRelation 都有一个关联的好友（这个 "好友" 也是一个用户）
                .WithMany()  // 好友不需要有 FriendRelations 属性（避免重复关系）
                .HasForeignKey(fr => fr.FriendId)  // 通过 FriendId 进行关联
                .OnDelete(DeleteBehavior.Cascade);  // 好友删除时，相关好友关系级联删除

            // 配置 Room 表（群聊房间）
            modelBuilder.Entity<Room>()
                .HasKey(r => r.Id);  // 房间的主键是 Id

            // 配置 Room 与 RoomMember 之间的关系（房间与成员）
            modelBuilder.Entity<RoomMember>()
                .HasKey(rm => new { rm.RoomId, rm.MemberId });  // 复合主键：房间ID + 成员ID

            modelBuilder.Entity<RoomMember>()
                .HasOne(rm => rm.Room)  // 每个 RoomMember 都有一个关联的房间
                .WithMany(r => r.RoomMembers)  // 一个房间可以有多个成员
                .HasForeignKey(rm => rm.RoomId)  // 通过 RoomId 进行关联
                .OnDelete(DeleteBehavior.Cascade);  // 房间删除时，成员关系级联删除

            modelBuilder.Entity<RoomMember>()
                .HasOne(rm => rm.Member)  // 每个 RoomMember 都有一个关联的成员（成员是一个用户）
                .WithMany(u => u.RoomMembers)  // 一个用户可以有多个房间成员关系
                .HasForeignKey(rm => rm.MemberId)  // 通过 MemberId 进行关联
                .OnDelete(DeleteBehavior.Cascade);  // 用户删除时，成员关系级联删除

            // 配置 Message 表（消息记录）
            modelBuilder.Entity<Message>()
                .HasKey(m => m.Id);  // 消息的主键是 Id

            modelBuilder.Entity<Message>()
                .HasOne(m => m.User)  // 每个 Message 都有一个关联的用户（消息发送者）
                .WithMany(u => u.Messages)  // 一个用户可以有多个消息
                .HasForeignKey(m => m.UserId)  // 通过 UserId 进行关联
                .OnDelete(DeleteBehavior.SetNull);  // 用户删除时，消息的 UserId 设置为 null

            // 配置 RoomMessage 表（房间消息与房间的关系）
            modelBuilder.Entity<RoomMessage>()
                .HasKey(rm => new { rm.RoomId, rm.MessageId });  // 复合主键：房间ID + 消息ID

            modelBuilder.Entity<RoomMessage>()
                .HasOne(rm => rm.Room)  // 每个 RoomMessage 都有一个关联的房间
                .WithMany(r => r.RoomMessages)  // 一个房间可以有多个消息
                .HasForeignKey(rm => rm.RoomId)  // 通过 RoomId 进行关联
                .OnDelete(DeleteBehavior.Cascade);  // 房间删除时，房间消息级联删除

            modelBuilder.Entity<RoomMessage>()
                .HasOne(rm => rm.Message)  // 每个 RoomMessage 都有一个关联的消息
                .WithMany(m => m.RoomMessages)  // 一个消息可以属于多个房间
                .HasForeignKey(rm => rm.MessageId)  // 通过 MessageId 进行关联
                .OnDelete(DeleteBehavior.Cascade);  // 消息删除时，房间消息级联删除

        }

        // 将驼峰命名（PascalCase）转换为小写字母加下划线（snake_case）
        private string ToSnakeCase(string name)
        {
            return string.Concat(
                name.Select((x, i) => i > 0 && char.IsUpper(x) ? "_" + x.ToString().ToLower() : x.ToString())
            ).ToLower();
        }

    }
}
