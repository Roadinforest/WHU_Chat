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


            // 房间实体配置
            modelBuilder.Entity<Room>(entity =>
            {
                //entity.ToTable("rooms"); // 可取消注释设置表名
                entity.HasKey(r => r.Id); // 设置主键为Id字段
                entity.Property(r => r.Name).IsRequired().HasMaxLength(255); // 房间名称必填，最大长度255
                entity.Property(r => r.AvatarUrl).HasMaxLength(255); // 头像URL，最大长度255
                entity.Property(r => r.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP"); // 创建时间默认当前时间

                // 与创建者(用户)的关系配置
                entity.HasOne(r => r.Creator)
                      .WithMany() // 如果User模型没有ICollection<Room> CreatedRooms属性时使用
                                  // .WithMany(u => u.CreatedRooms) // 当User有CreatedRooms属性时使用
                      .HasForeignKey(r => r.CreatorId) // 外键为CreatorId
                      .OnDelete(DeleteBehavior.Cascade); // 级联删除（根据业务也可设为Restrict/SetNull）

                // 与房间成员的一对多关系配置
                entity.HasMany(r => r.RoomMembers)
                      .WithOne(rm => rm.Room)
                      .HasForeignKey(rm => rm.RoomId)
                      .OnDelete(DeleteBehavior.Cascade); // 级联删除
            });

            // 房间成员实体配置（复合主键）
            modelBuilder.Entity<RoomMember>(entity =>
            {
                entity.ToTable("room_members"); // 设置表名与数据库匹配
                entity.HasKey(rm => new { rm.RoomId, rm.MemberId }); // 设置复合主键(RoomId + MemberId)

                // 与房间的多对一关系（已在Room实体中配置）
                // entity.HasOne(rm => rm.Room)
                //       .WithMany(r => r.RoomMembers)
                //       .HasForeignKey(rm => rm.RoomId);

                // 与用户(成员)的多对一关系配置
                entity.HasOne(rm => rm.Member)
                      .WithMany(u => u.RoomMembers) // User模型需要包含ICollection<RoomMember> RoomMembers属性
                      .HasForeignKey(rm => rm.MemberId) // 外键为MemberId
                      .OnDelete(DeleteBehavior.Cascade); // 级联删除

                // 额外配置
                entity.Property(rm => rm.Role)
                      .HasConversion<string>() // 将枚举转换为字符串存储
                      .HasDefaultValue(RoomMemberRole.MEMBER); // 默认角色为MEMBER
                entity.Property(rm => rm.JoinedAt)
                      .HasDefaultValueSql("CURRENT_TIMESTAMP"); // 加入时间默认当前时间
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
