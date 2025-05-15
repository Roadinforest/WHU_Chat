using System;
using System.ComponentModel.DataAnnotations;

namespace WHUChat.Server.Models
{

    public class User
    {
        public long Id { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string AvatarUrl { get; set; }
        public DateTime CreatedAt { get; set; }

        public ICollection<FriendRequest> SentFriendRequests { get; set; }
        public ICollection<FriendRequest> ReceivedFriendRequests { get; set; }
        public ICollection<FriendRelation> FriendRelations { get; set; }
        public ICollection<RoomMember> RoomMembers { get; set; }
        //public ICollection<Message> Messages { get; set; }
    }
}
