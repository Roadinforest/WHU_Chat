using System;

namespace WHUChat.Server.Models
{
    public class Room
    {
        public long Id { get; set; }
        public string? Name { get; set; }
        public string? AvatarUrl { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public ICollection<RoomMember> Members { get; set; } = new List<RoomMember>();
        public ICollection<Message> Messages { get; set; } = new List<Message>();
    }

}
