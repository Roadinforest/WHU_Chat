namespace WHUChat.Server.Models
{
    public class FriendRequest
    {
        public long SenderId { get; set; }
        public long ReceiverId { get; set; }
        public FriendRequestStatus Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? RespondedAt { get; set; }
        public User Sender { get; set; }
        public User Receiver { get; set; }
    }

    public enum FriendRequestStatus
    {
        PENDING,
        ACCEPTED,
        REJECTED
    }
}
