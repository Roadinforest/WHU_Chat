using System;

namespace WHUChat.Server.Models
{
    public class Room
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string AvatarUrl { get; set; }
        public DateTime CreatedAt { get; set; }

        public ICollection<RoomMember> RoomMembers { get; set; }
        public ICollection<RoomMessage> RoomMessages { get; set; }
    }

}
