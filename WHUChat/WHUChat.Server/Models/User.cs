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

        public ICollection<FriendRelation> FriendsAdded { get; set; } = new List<FriendRelation>();  // 我加的好友
        public ICollection<FriendRelation> FriendsWhoAddedMe { get; set; } = new List<FriendRelation>();  // 别人加我

        public ICollection<FriendRequest> SentRequests { get; set; } = new List<FriendRequest>();
        public ICollection<FriendRequest> ReceivedRequests { get; set; } = new List<FriendRequest>();
        public ICollection<RoomMember> RoomMemberships { get; set; } = new List<RoomMember>();
        public ICollection<Message> Messages { get; set; } = new List<Message>();
    }
}
