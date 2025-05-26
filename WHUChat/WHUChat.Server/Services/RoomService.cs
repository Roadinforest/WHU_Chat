using WHUChat.Server.DTOs.Room;
using WHUChat.Server.Models;
using WHUChat.Server.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging; // For logging

namespace WHUChat.Server.Services
{
    public class RoomService : IRoomService
    {
        private readonly IRoomRepository _roomRepository;
        private readonly ILogger<RoomService> _logger;

        public RoomService(IRoomRepository roomRepository, ILogger<RoomService> logger)
        {
            _roomRepository = roomRepository;
            _logger = logger;
        }

        public async Task<RoomResponseDto> CreateRoomAsync(long creatorId, CreateRoomRequestDto dto)
        {
            var creator = await _roomRepository.GetUserByIdAsync(creatorId);
            if (creator == null) throw new KeyNotFoundException("创建者用户不存在。");

            var room = new Room // 这是新的 Room 实体
            {
                Name = dto.Name,
                //AvatarUrl = dto.AvatarUrl,
                AvatarUrl = "https://www.dummyimage.com/600x400/000000/fff&text=" + dto.Name[0],
                CreatorId = creatorId,
                Creator = creator, // 设置导航属性
                CreatedAt = DateTime.UtcNow
                // Room.RoomMembers 集合已在模型中初始化为 new List<RoomMember>()
            };

            // 创建初始成员，并将其添加到 Room 的导航属性中
            var initialMember = new RoomMember
            {
                // Room = room; // EF Core 会通过 room.RoomMembers.Add(initialMember) 自动设置这个反向导航
                Member = creator, // 设置指向 User 的导航属性，EF Core 会从中推断 MemberId
                                  // MemberId = creatorId; // 也可以直接设置 MemberId，但设置 Member 导航属性更好

                Role = RoomMemberRole.ADMIN,
                JoinedAt = DateTime.UtcNow
            };
            room.RoomMembers.Add(initialMember); // 将初始成员添加到房间的成员列表

            // 将包含初始成员的 Room 对象传递给 Repository
            var persistedRoom = await _roomRepository.CreateRoomAsync(room);

            // 调用 SaveChangesAsync 一次性保存 Room 及其关联的 RoomMembers
            await _roomRepository.SaveChangesAsync();

            // 此处 persistedRoom.Id 已经被数据库正确填充
            // 并且 persistedRoom.RoomMembers.First().RoomId 也将被正确填充

            return new RoomResponseDto
            {
                Id = persistedRoom.Id,
                Name = persistedRoom.Name,
                AvatarUrl = persistedRoom.AvatarUrl,
                CreatedAt = persistedRoom.CreatedAt,
                Creator = new UserSimpleDto { Id = creator.Id, Username = creator.Username, AvatarUrl = creator.AvatarUrl },
                MemberCount = 1 // 或者: await _roomRepository.GetRoomMemberCountAsync(persistedRoom.Id)，这里简化处理
            };
        }

        public async Task JoinRoomAsync(long userId, long roomId)
        {
            var user = await _roomRepository.GetUserByIdAsync(userId);
            if (user == null) throw new KeyNotFoundException("用户不存在。");

            var room = await _roomRepository.GetRoomByIdAsync(roomId);
            if (room == null) throw new KeyNotFoundException("房间不存在。");

            if (await _roomRepository.IsUserMemberAsync(roomId, userId))
            {
                throw new InvalidOperationException("用户已经是该房间成员。");
            }

            var roomMember = new RoomMember { RoomId = roomId, MemberId = userId };
            // 这里设置 JoinedAt 和默认 Role
            roomMember.JoinedAt = DateTime.UtcNow;
            roomMember.Role = RoomMemberRole.MEMBER;
            await _roomRepository.AddMemberAsync(roomMember);
            await _roomRepository.SaveChangesAsync();
        }

        public async Task LeaveRoomAsync(long userId, long roomId)
        {
            var room = await _roomRepository.GetRoomByIdAsync(roomId);
            if (room == null) throw new KeyNotFoundException("房间不存在。");

            if (!await _roomRepository.IsUserMemberAsync(roomId, userId))
            {
                throw new InvalidOperationException("用户不是该房间成员。");
            }

            // TODO: 业务逻辑 - 如果房间创建者离开怎么办？如果房间只剩一人离开后是否删除房间？
            // 简单处理：直接移除成员
            bool removed = await _roomRepository.RemoveMemberAsync(roomId, userId);
            if (removed)
            {
                await _roomRepository.SaveChangesAsync();
            }
            else
            {
                _logger.LogWarning($"尝试移除用户 {userId} 从房间 {roomId}，但未找到成员记录 (IsUserMemberAsync 通过了)。");
                // 可能并发问题或者逻辑不一致，可以抛出异常或者仅记录
            }
            //// 如果房间创建者离开，且是最后一个成员，可以考虑删除房间
            //if (userId == room.CreatorId)
            //{
            //    var memberCount = await _roomRepository.GetRoomMemberCountAsync(roomId);
            //    if (memberCount == 0)
            //    {
            //         _context.Rooms.Remove(room); // 需要在 repo 实现删除房间的方法
            //         await _roomRepository.DeleteRoomAsync(room.Id);
            //         await _roomRepository.SaveChangesAsync();
            //        _logger.LogInformation($"房间 {roomId} 在创建者且最后一名成员离开后被删除。");
            //    }
            //}
        }

        public async Task<List<RoomResponseDto>> GetJoinedRoomsAsync(long userId)
        {
            var rooms = await _roomRepository.GetRoomsForUserAsync(userId);
            var roomResponseList = new List<RoomResponseDto>();

            foreach (var room in rooms)
            {
                roomResponseList.Add(new RoomResponseDto
                {
                    Id = room.Id,
                    Name = room.Name,
                    AvatarUrl = room.AvatarUrl,
                    CreatedAt = room.CreatedAt,
                    Creator = new UserSimpleDto { Id = room.Creator.Id, Username = room.Creator.Username, AvatarUrl = room.Creator.AvatarUrl },
                    MemberCount = await _roomRepository.GetRoomMemberCountAsync(room.Id) // 每次都查一次数量，可优化
                });
            }
            return roomResponseList;
        }

        public async Task<List<RoomMemberResponseDto>> GetRoomMembersAsync(long roomId, long requestingUserId)
        {
            var room = await _roomRepository.GetRoomByIdAsync(roomId);
            if (room == null) throw new KeyNotFoundException("房间不存在。");

            // 权限检查：只有房间成员才能查看成员列表 (可根据业务调整)
            if (!await _roomRepository.IsUserMemberAsync(roomId, requestingUserId))
            {
                throw new UnauthorizedAccessException("无权查看该房间的成员列表。");
            }

            var members = await _roomRepository.GetMembersInRoomAsync(roomId);
            return members.Select(m => new RoomMemberResponseDto
            {
                UserId = m.Member.Id,
                Username = m.Member.Username,
                AvatarUrl = m.Member.AvatarUrl,
                JoinedAt = m.JoinedAt, 
                Role = m.Role.ToString() 
            }).ToList();
        }

        public async Task InviteUserToRoomAsync(long inviterId, long roomId, long invitedUserId)
        {
            var inviter = await _roomRepository.GetUserByIdAsync(inviterId);
            if (inviter == null) throw new KeyNotFoundException("邀请者用户不存在。");

            var invitedUser = await _roomRepository.GetUserByIdAsync(invitedUserId);
            if (invitedUser == null) throw new KeyNotFoundException("被邀请的用户不存在。");

            var room = await _roomRepository.GetRoomByIdAsync(roomId);
            if (room == null) throw new KeyNotFoundException("房间不存在。");

            // 权限检查：邀请者必须是房间成员
            if (!await _roomRepository.IsUserMemberAsync(roomId, inviterId))
            {
                throw new UnauthorizedAccessException("邀请者不是该房间成员，无权邀请。");
            }

            if (await _roomRepository.IsUserMemberAsync(roomId, invitedUserId))
            {
                throw new InvalidOperationException("被邀请的用户已经是该房间成员。");
            }

            // 直接添加成员 (简单邀请方式)
            var roomMember = new RoomMember { RoomId = roomId, MemberId = invitedUserId };
            await _roomRepository.AddMemberAsync(roomMember);
            await _roomRepository.SaveChangesAsync();

            // TODO: 更完善的邀请机制会创建一个邀请记录，等待被邀请者接受。
            // 这需要额外的 room_invitations 表和相关逻辑。
        }
    }
}