using WHUChat.Server.Models; // Assuming FriendRequestStatus is here

namespace WHUChat.Server.DTOs.FriendShip
{
    // 发送好友请求 DTO 
    public class SendFriendRequestDto
    {
        // public long SenderId { get; set; } // SenderId 通常从 Token 获取，不需要 DTO 包含
        public long ReceiverId { get; set; }
    }

    // 响应好友请求 DTO
    public class RespondFriendRequestDto
    {
        public long SenderId { get; set; } // 指明是哪个用户发送的请求需要响应
        // public long ReceiverId { get; set; } // ReceiverId 通常是当前用户，从 Token 获取
        public bool Accept { get; set; } // true = 接受, false = 拒绝
    }

    // 用于显示好友列表的 DTO
    public class FriendDto
    {
        public long Id { get; set; }
        public string Username { get; set; }
        public string? AvatarUrl { get; set; }
        public DateTime FriendedAt { get; set; } // 成为好友的时间
    }

    // 用于显示好友请求列表的 DTO
    public class FriendRequestDto
    {
        public long RequestId { get; set; } // 可以用 SenderId + ReceiverId 唯一标识
        public long SenderId { get; set; }
        public string SenderUsername { get; set; }
        public string? SenderAvatarUrl { get; set; }
        public long ReceiverId { get; set; }
        public string ReceiverUsername { get; set; }
        public string? ReceiverAvatarUrl { get; set; }
        public FriendRequestStatus Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? RespondedAt { get; set; }
    }
}