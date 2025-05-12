namespace WHUChat.Server.Models
{
    public class FriendRelation
    {
        public long UserId { get; set; }
        public User User { get; set; } = null!;
        public long FriendId { get; set; }
        public User Friend { get; set; } = null!;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
