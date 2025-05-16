namespace WHUChat.Server.DTOs
{
    public class SendMessageRequestDto
    {
        public string UserName { get; set; }
        public string? Content { get; set; }
        public string? ResUrl { get; set; }
    }
}
