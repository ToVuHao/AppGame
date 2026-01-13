using GameApp.API.Data;
using GameApp.API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GameApp.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FriendsController : ControllerBase
    {
        private readonly GameDbContext _context;

        public FriendsController(GameDbContext context)
        {
            _context = context;
        }

        // 1. GỬI LỜI MỜI KẾT BẠN
        [HttpPost("send")]
        public async Task<IActionResult> SendRequest([FromBody] FriendshipRequestDto req)
        {
            if (req.SenderId == req.ReceiverId) return BadRequest("Không thể tự kết bạn!");

            // Kiểm tra xem đã có lời mời nào chưa (dù chiều nào)
            var exists = await _context.Friendships.AnyAsync(f =>
                ((f.SenderId == req.SenderId && f.ReceiverId == req.ReceiverId) ||
                 (f.SenderId == req.ReceiverId && f.ReceiverId == req.SenderId)));

            if (exists) return BadRequest("Đã có lời mời hoặc đã là bạn bè.");

            var friendship = new Friendship
            {
                SenderId = req.SenderId,
                ReceiverId = req.ReceiverId,
                Status = "Pending"
            };

            _context.Friendships.Add(friendship);
            await _context.SaveChangesAsync();
            return Ok("Đã gửi lời mời!");
        }

        // 2. CHẤP NHẬN LỜI MỜI
        [HttpPost("accept/{id}")]
        public async Task<IActionResult> AcceptRequest(int id) // id là Friendship Id
        {
            var friendship = await _context.Friendships.FindAsync(id);
            if (friendship == null) return NotFound();

            friendship.Status = "Accepted";
            await _context.SaveChangesAsync();
            return Ok("Đã trở thành bạn bè!");
        }

        // 3. TỪ CHỐI / HỦY KẾT BẠN
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteFriendship(int id)
        {
            var f = await _context.Friendships.FindAsync(id);
            if (f != null)
            {
                _context.Friendships.Remove(f);
                await _context.SaveChangesAsync();
            }
            return Ok("Đã xóa.");
        }

        // 4. LẤY DANH SÁCH BẠN BÈ (Của user cụ thể)
        [HttpGet("list/{userId}")]
        public async Task<IActionResult> GetFriends(int userId)
        {
            var friends = await _context.Friendships
                .Where(f => (f.SenderId == userId || f.ReceiverId == userId) && f.Status == "Accepted")
                .Include(f => f.Sender)
                .Include(f => f.Receiver)
                .Select(f => new
                {
                    FriendshipId = f.Id,
                    FriendId = f.SenderId == userId ? f.ReceiverId : f.SenderId,
                    FriendName = f.SenderId == userId ? f.Receiver!.FullName : f.Sender!.FullName,
                    FriendUsername = f.SenderId == userId ? f.Receiver!.Username : f.Sender!.Username
                })
                .ToListAsync();

            return Ok(friends);
        }

        // 5. LẤY DANH SÁCH LỜI MỜI ĐANG CHỜ (Người khác gửi cho mình)
        [HttpGet("requests/{userId}")]
        public async Task<IActionResult> GetPendingRequests(int userId)
        {
            var requests = await _context.Friendships
                .Where(f => f.ReceiverId == userId && f.Status == "Pending")
                .Include(f => f.Sender)
                .Select(f => new
                {
                    FriendshipId = f.Id,
                    SenderId = f.SenderId,
                    SenderName = f.Sender!.FullName
                })
                .ToListAsync();

            return Ok(requests);
        }

        // 6. TÌM NGƯỜI DÙNG ĐỂ KẾT BẠN (Trừ chính mình và admin)
        [HttpGet("find/{currentUserId}")]
        public async Task<IActionResult> FindUsers(int currentUserId)
        {
            var users = await _context.Users
                .Where(u => u.Id != currentUserId && u.Role != "admin")
                .Select(u => new { u.Id, u.FullName, u.Username })
                .ToListAsync();
            return Ok(users);
        }
    }

    public class FriendshipRequestDto
    {
        public int SenderId { get; set; }
        public int ReceiverId { get; set; }
    }
}