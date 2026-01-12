using GameApp.API.Data;
using GameApp.API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GameApp.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly GameDbContext _context;

        public AuthController(GameDbContext context)
        {
            _context = context;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] User user)
        {
            // Kiểm tra tài khoản tồn tại
            if (await _context.Users.AnyAsync(u => u.Username == user.Username))
                return BadRequest("Tài khoản đã tồn tại");

            // BẢO MẬT: Mặc định đăng ký mới luôn là "user"
            // (Chỉ sửa thành "admin" trực tiếp trong Database)
            user.Role = "user";

            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return Ok("Đăng ký thành công");
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] User loginInfo)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Username == loginInfo.Username && u.Password == loginInfo.Password);

            if (user == null)
                return Unauthorized("Sai tài khoản hoặc mật khẩu");

            // QUAN TRỌNG: Trả về Role để Flutter phân quyền
            return Ok(new
            {
                Message = "Đăng nhập thành công",
                UserId = user.Id,
                FullName = user.FullName,
                Role = user.Role // <-- Dòng này giúp App biết vào Admin hay Home
            });
        }
    }
}