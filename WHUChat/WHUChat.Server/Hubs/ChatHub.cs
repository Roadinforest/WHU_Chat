using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using WHUChat.Server.Models;
using WHUChat.Server.Services;

namespace WHUChat.Server.Hubs
{
    [Authorize]
    public class ChatHub:Hub
    {
        private readonly IChatService _chatService;
        private readonly IUserService _userService;
        private Dictionary<string, long> userDic;

        public ChatHub( IChatService chatService,IUserService userService)
        {
            _chatService = chatService;
            _userService = userService;
            userDic = new Dictionary<string, long>();
        }

        public override async Task OnConnectedAsync()//登录(调用[HttpPost("login")])后开启连接
        {
            long userId = long.Parse(this.Context.User!.FindFirst(ClaimTypes.NameIdentifier)!.Value);//从token中获取ID
            userDic.Add(this.Context.ConnectionId, userId);
            await base.OnConnectedAsync();
        }

        public override Task OnDisconnectedAsync(Exception? exception)
        {
            userDic.Remove(this.Context.ConnectionId);
            return base.OnDisconnectedAsync(exception);
        }

        public async Task SendMessage(string roomId, string message,string? resUrl)
        {
            long userId = userDic.GetValueOrDefault(this.Context.ConnectionId);
            User u = _userService.GetById(userId);
            Message m = new Message
            {
                RoomId = long.Parse(roomId),
                UserName = u.Username,
                Content = message,
                CreatedAt = DateTime.Now,
                ResUrl = resUrl,
            };
            await _chatService.InsertMessage(m);//将消息加入数据库用与历史浏览
            await Clients.Group(roomId).SendAsync("ReceiveMessage", u.Username, message,resUrl);//根据roomId显示到对应房间界面


        }


        //在[HttpGet("{roomId}/get_history")]后调用，将连接加入群组
        public async Task EntryRoom(string roomId)
        {

            await Groups.AddToGroupAsync(Context.ConnectionId, roomId);
        }


        //退出房间时调用
        public async Task QuitFromRoom(string roomId)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, roomId);
        }
    }
}
