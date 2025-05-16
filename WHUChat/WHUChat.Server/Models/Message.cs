using System;

namespace WHUChat.Server.Models
{
    public class Message
    {
        public long Id { get; set; }
        public long UserId { get; set; }
        public DateTime CreatedAt { get; set; }
        public string Content { get; set; }
        public string ResUrl { get; set; }
        public User User { get; set; }
    }
}
