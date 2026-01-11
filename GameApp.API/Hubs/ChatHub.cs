using GameApp.API.Data;
using GameApp.API.Models;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using System.Linq;

namespace GameApp.API.Hubs
{
    public class ChatHub : Hub
    {
        private readonly GameDbContext _context;

        // Inject Database vào Hub để dùng
        public ChatHub(GameDbContext context)
        {
            _context = context;
        }

        // HÀM 1: Khi có người vừa kết nối vào phòng Chat
        public override async Task OnConnectedAsync()
        {
            // Lấy 50 tin nhắn gần nhất từ Database
            var history = await _context.ChatMessages
                .OrderByDescending(m => m.Timestamp) // Lấy mới nhất
                .Take(50)                            // Chỉ lấy 50 tin
                .OrderBy(m => m.Timestamp)           // Sắp xếp lại theo thứ tự thời gian để hiển thị xuôi
                .ToListAsync();

            // Gửi từng tin nhắn cũ cho người vừa vào (Clients.Caller)
            foreach (var msg in history)
            {
                await Clients.Caller.SendAsync("ReceiveMessage", msg.UserName, msg.Content);
            }

            await base.OnConnectedAsync();
        }

        // HÀM 2: Khi người dùng gửi tin nhắn mới
        public async Task SendMessage(string user, string message)
        {
            // 1. Lưu vào Database
            var chatMsg = new ChatMessage
            {
                UserName = user,
                Content = message
            };
            _context.ChatMessages.Add(chatMsg);
            await _context.SaveChangesAsync();

            // 2. Gửi cho TẤT CẢ mọi người (như cũ)
            await Clients.All.SendAsync("ReceiveMessage", user, message);
        }
    }
}