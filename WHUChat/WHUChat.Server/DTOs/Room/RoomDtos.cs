using System.ComponentModel.DataAnnotations;

namespace WHUChat.Server.DTOs.Room
{
    public class CreateRoomRequestDto
    {
        [Required]
        [StringLength(100, MinimumLength = 3)]
        public string Name { get; set; }

        [StringLength(255)]
        public string? AvatarUrl { get; set; }
    }

    public class RoomResponseDto
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string? AvatarUrl { get; set; }
        public DateTime CreatedAt { get; set; }
        public UserSimpleDto Creator { get; set; } // 创建者信息
        public int MemberCount { get; set; }
        public List<RoomMemberResponseDto>? Members { get; set; } // 按需包含成员列表
    }

    // 辅助 DTO，避免循环引用或过多信息
    public class UserSimpleDto
    {
        public long Id { get; set; }
        public string Username { get; set; }
        public string? AvatarUrl { get; set; }
    }

    public class RoomMemberResponseDto
    {
        public long UserId { get; set; }
        public string Username { get; set; }
        public string? AvatarUrl { get; set; }
        public DateTime JoinedAt { get; set; } 
        public string Role { get; set; }
    }

    public class InviteUserToRoomRequestDto
    {
        [Required]
        public long InvitedUserId { get; set; }
    }
}