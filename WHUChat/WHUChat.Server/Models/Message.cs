using System;

namespace WHUChat.Server.Models
{
    public class Message
    {
        public long Id { get; set; }
        public long UserId { get; set; }
        public User User { get; set; } = null!;
        public long RoomId { get; set; }
        public Room Room { get; set; } = null!;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public string? Content { get; set; }
        public string? ResUrl { get; set; }
    }
}
