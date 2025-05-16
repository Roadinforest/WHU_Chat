namespace WHUChat.Server.Models
{
    public class FriendRelation
    {
        public long UserId { get; set; }
        public long FriendId { get; set; }

        public long RoomId { get; set; }
        public DateTime CreatedAt { get; set; }
        public User User { get; set; }
        public User Friend { get; set; }
    }
}
