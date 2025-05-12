namespace WHUChat.Server.DTOs
{
    public class UserDto
    {
        public long Id { get; set; }
        public string Username { get; set; } = null!;
        public string? AvatarUrl { get; set; }
    }
}