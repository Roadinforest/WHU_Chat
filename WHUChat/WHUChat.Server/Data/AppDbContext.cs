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

            modelBuilder.Entity<FriendRequest>()
                 .HasKey(fr => new { fr.SenderId, fr.ReceiverId });

            // 显式配置枚举映射为字符串
            modelBuilder.Entity<FriendRequest>()
                .Property(fr => fr.Status)
                .HasConversion<string>();

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


            // Room Configuration
            modelBuilder.Entity<Room>(entity =>
            {
                //entity.ToTable("rooms"); // 与你的 SQL 匹配
                entity.HasKey(r => r.Id);
                entity.Property(r => r.Name).IsRequired().HasMaxLength(255);
                entity.Property(r => r.AvatarUrl).HasMaxLength(255);
                entity.Property(r => r.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");

                // Relation to Creator (User)
                entity.HasOne(r => r.Creator)
                      .WithMany() // 如果 User 模型没有 ICollection<Room> CreatedRooms 属性
                                  // .WithMany(u => u.CreatedRooms) // 如果 User 有 CreatedRooms 属性
                      .HasForeignKey(r => r.CreatorId)
                      .OnDelete(DeleteBehavior.Cascade); // 或 Restrict/SetNull，取决于业务

                // Relation to RoomMembers (One-to-Many)
                entity.HasMany(r => r.RoomMembers)
                      .WithOne(rm => rm.Room)
                      .HasForeignKey(rm => rm.RoomId)
                      .OnDelete(DeleteBehavior.Cascade);
            });


            // RoomMember Configuration (Composite Key)
            modelBuilder.Entity<RoomMember>(entity =>
            {
                entity.ToTable("room_members"); // 与你的 SQL 匹配
                entity.HasKey(rm => new { rm.RoomId, rm.MemberId }); // 复合主键

                // Relation to Room (Many-to-One)
                // (already configured by Room's HasMany)
                // entity.HasOne(rm => rm.Room)
                //       .WithMany(r => r.RoomMembers)
                //       .HasForeignKey(rm => rm.RoomId);

                // Relation to User (Member) (Many-to-One)
                entity.HasOne(rm => rm.Member)
                      .WithMany(u => u.RoomMembers) // User 模型需要 ICollection<RoomMember> RoomMembers
                      .HasForeignKey(rm => rm.MemberId)
                      .OnDelete(DeleteBehavior.Cascade);

                //如果采纳了数据库建议，可以配置 Role 和 JoinedAt
                entity.Property(rm => rm.Role).HasConversion<string>().HasDefaultValue(RoomMemberRole.MEMBER);
                entity.Property(rm => rm.JoinedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
            });


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
