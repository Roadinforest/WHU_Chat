namespace WHUChat.Server.Models
{

    public class RoomMember
    {
        public long RoomId { get; set; }
        public Room Room { get; set; } = null!;
        public long MemberId { get; set; }
        public User Member { get; set; } = null!;
    }
}
