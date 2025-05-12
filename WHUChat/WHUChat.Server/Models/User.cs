using System;
using System.ComponentModel.DataAnnotations;

namespace WHUChat.Server.Models
{

    public class User
    {
        public long Id { get; set; }


        [Required]
        public string Username { get; set; } = null!;

        [Required]
        public string Password { get; set; } = null!;

        public string? AvatarUrl { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public ICollection<FriendRelation> Friends { get; set; } = new List<FriendRelation>();
        public ICollection<FriendRequest> SentRequests { get; set; } = new List<FriendRequest>();
        public ICollection<FriendRequest> ReceivedRequests { get; set; } = new List<FriendRequest>();
        public ICollection<RoomMember> RoomMemberships { get; set; } = new List<RoomMember>();
        public ICollection<Message> Messages { get; set; } = new List<Message>();
    }
}
