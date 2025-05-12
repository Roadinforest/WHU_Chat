namespace WHUChat.Server.DTOs
{
    public class RoomDto
    {
        public long Id { get; set; }
        public string? Name { get; set; }
        public string? AvatarUrl { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
