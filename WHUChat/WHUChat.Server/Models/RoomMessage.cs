namespace WHUChat.Server.Models
{
    public class RoomMessage
    {
        public long RoomId { get; set; }
        public long MessageId { get; set; }

        public Room Room { get; set; }
        public Message Message { get; set; }
    }
}
