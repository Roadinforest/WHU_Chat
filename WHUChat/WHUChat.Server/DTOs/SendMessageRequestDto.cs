namespace WHUChat.Server.DTOs
{
    public class SendMessageRequestDto
    {
        public long Id { get; set; }
        public string UserName { get; set; }
        public string? Content { get; set; }
        public string? ResUrl { get; set; }
    }
}
