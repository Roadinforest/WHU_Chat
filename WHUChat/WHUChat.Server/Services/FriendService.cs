using WHUChat.Server.DTOs.FriendShip;
using WHUChat.Server.Models;
using WHUChat.Server.Repositories;

namespace WHUChat.Server.Services
{
    public class FriendService : IFriendService
    {
        private readonly IFriendshipRepository _friendshipRepository;
        // private readonly IUserRepository _userRepository; // 如果有单独的 User Repository

        public FriendService(IFriendshipRepository friendshipRepository) // 注入 Repository
        {
            _friendshipRepository = friendshipRepository;
        }

        public async Task SendFriendRequestAsync(long senderId, long receiverId)
        {
            // 1. 验证: 不能给自己发送请求
            if (senderId == receiverId)
            {
                throw new ArgumentException("不能向自己发送好友请求。");
            }

            // 2. 验证: 接收者是否存在 (可选，取决于你的用户查找逻辑在哪里实现)
            var receiver = await _friendshipRepository.GetUserByIdAsync(receiverId);
            if (receiver == null)
            {
                throw new KeyNotFoundException("接收用户不存在。");
            }
            var sender = await _friendshipRepository.GetUserByIdAsync(senderId);
            if (sender == null)
            {
                //理论上sender是当前用户，不可能不存在，除非token失效或用户被删除
                throw new KeyNotFoundException("发送用户不存在。");
            }


            // 3. 验证: 是否已经是好友
            if (await _friendshipRepository.AreUsersFriendsAsync(senderId, receiverId))
            {
                throw new InvalidOperationException("你们已经是好友了。");
            }

            // 4. 验证: 是否已存在待处理或已接受的请求 (双向检查)
            var existingRequest = await _friendshipRepository.GetFriendRequestAsync(senderId, receiverId);
            if (existingRequest != null && (existingRequest.Status == FriendRequestStatus.PENDING || existingRequest.Status == FriendRequestStatus.ACCEPTED))
            {
                throw new InvalidOperationException("已发送过好友请求。");
            }
            var reverseRequest = await _friendshipRepository.GetFriendRequestAsync(receiverId, senderId);
            if (reverseRequest != null && (reverseRequest.Status == FriendRequestStatus.PENDING || reverseRequest.Status == FriendRequestStatus.ACCEPTED))
            {
                throw new InvalidOperationException("对方已向你发送过好友请求，请处理。");
            }


            // 5. 创建并保存好友请求
            var friendRequest = new FriendRequest
            {
                SenderId = senderId,
                ReceiverId = receiverId,
                Status = FriendRequestStatus.PENDING,
                CreatedAt = DateTime.UtcNow // 建议使用 UTC 时间
                // RespondedAt = null (默认)
            };

            await _friendshipRepository.AddFriendRequestAsync(friendRequest);
            await _friendshipRepository.SaveChangesAsync(); // 保存更改
        }

        public async Task RespondFriendRequestAsync(long receiverId, long senderId, bool accept)
        {
            // 1. 获取请求
            var request = await _friendshipRepository.GetFriendRequestAsync(senderId, receiverId);

            // 2. 验证: 请求是否存在
            if (request == null)
            {
                throw new KeyNotFoundException("好友请求不存在。");
            }

            // 3. 验证: 请求是否发给当前用户 (receiverId)
            if (request.ReceiverId != receiverId)
            {
                throw new UnauthorizedAccessException("无权响应此好友请求。");
            }

            // 4. 验证: 请求是否是 PENDING 状态
            if (request.Status != FriendRequestStatus.PENDING)
            {
                throw new InvalidOperationException($"此请求已被处理过 (状态: {request.Status})。");
            }

            // 5. 处理请求
            request.Status = accept ? FriendRequestStatus.ACCEPTED : FriendRequestStatus.REJECTED;
            request.RespondedAt = DateTime.UtcNow; // 记录响应时间

            await _friendshipRepository.UpdateFriendRequestAsync(request); // 标记请求为更新状态

            // 6. 如果接受，添加双向好友关系
            if (accept)
            {
                // 检查是否已经是好友（防止并发问题或逻辑错误）
                if (await _friendshipRepository.AreUsersFriendsAsync(senderId, receiverId))
                {
                    // 如果已经是好友，可能只需更新请求状态即可，或者抛出异常
                    await _friendshipRepository.SaveChangesAsync(); // 仅保存请求状态的更新
                                                                    // 可以选择记录日志或抛出特定异常
                    Console.WriteLine($"Warning: Users {senderId} and {receiverId} were already friends when accepting request.");
                    return; // 或者根据业务逻辑决定是否继续
                }

                var relation1 = new FriendRelation { UserId = senderId, FriendId = receiverId, CreatedAt = DateTime.UtcNow };
                var relation2 = new FriendRelation { UserId = receiverId, FriendId = senderId, CreatedAt = DateTime.UtcNow };

                await _friendshipRepository.AddFriendRelationAsync(relation1);
                await _friendshipRepository.AddFriendRelationAsync(relation2);
            }

            // 7. 保存所有更改 (更新请求状态，可能还包括添加好友关系)
            await _friendshipRepository.SaveChangesAsync();
        }

        public async Task<List<FriendRequestDto>> GetReceivedRequestsAsync(long userId)
        {
            var requests = await _friendshipRepository.GetReceivedPendingRequestsAsync(userId);

            // 映射到 DTO
            return requests.Select(req => new FriendRequestDto
            {
                // RequestId = req.Id, // 如果 FriendRequest 表有自增 ID
                SenderId = req.SenderId,
                SenderUsername = req.Sender?.Username ?? "未知用户", // 从 Include 的 Sender 获取
                SenderAvatarUrl = req.Sender?.AvatarUrl,
                ReceiverId = req.ReceiverId,
                // ReceiverUsername = req.Receiver?.Username, // Receiver 就是当前用户，信息通常已知
                // ReceiverAvatarUrl = req.Receiver?.AvatarUrl,
                Status = req.Status,
                CreatedAt = req.CreatedAt,
                RespondedAt = req.RespondedAt
            }).ToList();
        }

        public async Task<List<FriendRequestDto>> GetSentRequestsAsync(long userId)
        {
            var requests = await _friendshipRepository.GetSentRequestsAsync(userId);
            var currentUser = await _friendshipRepository.GetUserByIdAsync(userId); // 获取当前用户信息


            // 映射到 DTO
            return requests.Select(req => new FriendRequestDto
            {
                SenderId = req.SenderId,
                // SenderUsername = req.Sender?.Username, // Sender 是当前用户
                SenderUsername = currentUser?.Username ?? "我", // 明确是当前用户
                SenderAvatarUrl = currentUser?.AvatarUrl,
                ReceiverId = req.ReceiverId,
                ReceiverUsername = req.Receiver?.Username ?? "未知用户", // 从 Include 的 Receiver 获取
                ReceiverAvatarUrl = req.Receiver?.AvatarUrl,
                Status = req.Status,
                CreatedAt = req.CreatedAt,
                RespondedAt = req.RespondedAt
            }).ToList();
        }


        public async Task<List<FriendDto>> GetFriendListAsync(long userId)
        {
            var relations = await _friendshipRepository.GetFriendRelationsAsync(userId);

            // 映射到 DTO
            return relations.Select(rel => new FriendDto
            {
                Id = rel.FriendId, // 好友的 ID
                Username = rel.Friend?.Username ?? "未知好友", // 从 Include 的 Friend 获取
                AvatarUrl = rel.Friend?.AvatarUrl,
                FriendedAt = rel.CreatedAt // 好友关系创建时间
            }).ToList();
        }
        public async Task DeleteFriendAsync(long userId, long friendId)
        {
            if (userId == friendId) throw new ArgumentException("不能删除自己作为好友。");

            var friendUser = await _friendshipRepository.GetUserByIdAsync(friendId);
            if (friendUser == null) throw new KeyNotFoundException("指定的好友用户不存在。");

            // 检查是否是好友 (通过查找关系)
            // (或者可以直接尝试删除，然后判断是否有更改)
            var relationExists = await _friendshipRepository.AreUsersFriendsAsync(userId, friendId); // 使用之前的检查方法
            if (!relationExists)
            {
                throw new InvalidOperationException("你们不是好友关系。");
            }


            // 1. 标记好友关系记录为删除 (双向)
            // 注意：DeleteFriendRelationsAsync 现在内部处理查找和标记删除
            await _friendshipRepository.DeleteFriendRelationsAsync(userId, friendId);


            // 2. --- 新增：删除这两个用户之间的所有好友请求记录 ---
            await _friendshipRepository.DeleteFriendRequestsBetweenAsync(userId, friendId);


            // 3. 保存所有更改 (删除关系 + 删除请求) 到数据库
            // EF Core 会将上面两个步骤中标记的所有删除操作在一个事务中执行
            await _friendshipRepository.SaveChangesAsync();
        }

    }
}

