namespace WHUChat.Server.Models
{
    public class FriendRequest
    {
        public long Id { get; set; }
        public long SenderId { get; set; }
        public User Sender { get; set; } = null!;
        public long ReceiverId { get; set; }
        public User Receiver { get; set; } = null!;
        public string Status { get; set; } = "PENDING"; // PENDING, ACCEPTED, REJECTED
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? RespondedAt { get; set; }
    }
}
