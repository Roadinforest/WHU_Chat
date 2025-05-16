namespace WHUChat.Server.Models
{
    public class FriendRelation
    {
        public long UserId { get; set; }
        public long FriendId { get; set; }
<<<<<<< HEAD

=======
>>>>>>> 6da43cb152d7d6eda20fd386dc8e8aee1fc52581
        public long RoomId { get; set; }
        public DateTime CreatedAt { get; set; }
        public User User { get; set; }
        public User Friend { get; set; }

    }
}
