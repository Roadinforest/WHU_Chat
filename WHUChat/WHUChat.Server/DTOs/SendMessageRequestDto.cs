namespace WHUChat.Server.DTOs
{
    public class SendMessageRequestDto
    {
        public long RoomId { get; set; }
        public string? Content { get; set; }
        public string? ResUrl { get; set; }
    }
}
