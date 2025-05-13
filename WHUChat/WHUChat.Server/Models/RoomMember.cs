using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace WHUChat.Server.Models
{
    // 代表用户和房间的多对多关系
    public class RoomMember
    {
        public long RoomId { get; set; }
        public Room Room { get; set; }

        public long MemberId { get; set; }
        public User Member { get; set; }

        public RoomMemberRole Role { get; set; } = RoomMemberRole.MEMBER;
        public DateTime JoinedAt { get; set; } = DateTime.UtcNow;
    }
     public enum RoomMemberRole
    {
        MEMBER,
        ADMIN
    }
}