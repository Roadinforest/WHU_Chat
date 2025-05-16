using System;
using System.ComponentModel.DataAnnotations;

namespace WHUChat.Server.Models
{
    public class Message
    {
        public long Id { get; set; }
<<<<<<< HEAD
        public long UserId { get; set; }
        public DateTime CreatedAt { get; set; }
        public string Content { get; set; }
        public string ResUrl { get; set; }
        public User User { get; set; }
=======

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
>>>>>>> 47c106626373e7dbc533ef5e049aa6dc45e7f374
    }
}
