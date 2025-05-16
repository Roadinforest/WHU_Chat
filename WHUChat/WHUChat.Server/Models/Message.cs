using System;
using System.ComponentModel.DataAnnotations;

namespace WHUChat.Server.Models
{
    public class Message
    {
        public long Id { get; set; }

        public long RoomId { get; set; }

        [Required]
        [StringLength(255)]
        public string Username { get; set; }

        [StringLength(255)]
        public string Content { get; set; }

        public DateTime CreatedAt { get; set; }
        public string? Resurl { get; set; }

        //public User User { get; set; }

        public Room Room { get; set; }
    }
}