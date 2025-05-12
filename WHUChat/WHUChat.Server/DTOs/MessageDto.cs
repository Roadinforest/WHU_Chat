namespace WHUChat.Server.DTOs
{
    public class MessageDto
    {
        public long Id { get; set; }
        public long RoomId { get; set; }
        public long UserId { get; set; }
        public string? Content { get; set; }
        public string? ResUrl { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}