using WHUChat.Server.Models;

namespace WHUChat.Server.DTOs
{
    public class FriendRelationDto
    {
        public long UserId { get; set; }
        public long FriendId { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
