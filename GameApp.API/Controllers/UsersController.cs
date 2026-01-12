using GameApp.API.Data;
using GameApp.API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GameApp.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly GameDbContext _context;

        public UsersController(GameDbContext context)
        {
            _context = context;
        }

        // 1. LẤY DANH SÁCH
        [HttpGet]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await _context.Users.ToListAsync();
            return Ok(users);
        }

        // 2. THÊM NGƯỜI DÙNG MỚI (Admin tạo)
        [HttpPost]
        public async Task<IActionResult> CreateUser([FromBody] User user)
        {
            if (await _context.Users.AnyAsync(u => u.Username == user.Username))
                return BadRequest("Tên đăng nhập đã tồn tại!");

            // Nếu admin không nhập role, mặc định là user
            if (string.IsNullOrEmpty(user.Role)) user.Role = "user";

            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return Ok(user);
        }

        // 3. SỬA NGƯỜI DÙNG
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(int id, [FromBody] User updatedUser)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null) return NotFound("Không tìm thấy người dùng");

            // Cập nhật thông tin
            user.FullName = updatedUser.FullName;
            user.Password = updatedUser.Password;
            user.Role = updatedUser.Role; // Admin có quyền sửa cả Role

            await _context.SaveChangesAsync();
            return Ok(user);
        }

        // 4. XÓA NGƯỜI DÙNG
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null) return NotFound("Không tìm thấy người dùng");

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();

            return Ok("Đã xóa thành công!");
        }
    }
}