using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WHUChat.Server.Models
{
    public class Room
    {
        public long Id { get; set; }

        [Required]
        [StringLength(255)]
        public string Name { get; set; }

        [StringLength(255)]
        public string? AvatarUrl { get; set; }

        public long CreatorId { get; set; } // 房间的创建者
        public User Creator { get; set; } // 导航属性

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public ICollection<RoomMember> RoomMembers { get; set; } = new List<RoomMember>();
        public ICollection<Message> Messages { get; set; } // 如果有房间消息的话
    }
}