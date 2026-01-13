using GameApp.API.Data;
using GameApp.API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net;
using System.Net.Mail;

namespace GameApp.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly GameDbContext _context;
        private readonly IConfiguration _configuration;

        public AuthController(GameDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        // --- 1. ĐĂNG KÝ ---
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] User user)
        {
            if (await _context.Users.AnyAsync(u => u.Username == user.Username))
                return BadRequest("Tài khoản đã tồn tại");

            user.Role = "user";

            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return Ok("Đăng ký thành công");
        }

        // --- 2. ĐĂNG NHẬP ---
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] User loginInfo)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Username == loginInfo.Username && u.Password == loginInfo.Password);

            if (user == null)
                return Unauthorized("Sai tài khoản hoặc mật khẩu");

            return Ok(new
            {
                Message = "Đăng nhập thành công",
                UserId = user.Id,
                FullName = user.FullName,
                Role = user.Role
            });
        }

        // --- 3. QUÊN MẬT KHẨU (Gửi OTP) ---
        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPassRequest request)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == request.Username); // Giả định Username là Email
            if (user == null) return NotFound("Tài khoản không tồn tại");

            // Tạo mã OTP 6 số
            Random random = new Random();
            string otp = random.Next(100000, 999999).ToString();

            // Lưu vào DB
            user.OtpCode = otp;
            user.OtpExpiryTime = DateTime.Now.AddMinutes(5);
            await _context.SaveChangesAsync();

            // Gửi Email
            string subject = "Mã xác thực Quên mật khẩu - Game App";
            string body = $"Xin chào {user.FullName},\n\nMã OTP của bạn là: {otp}\nMã này sẽ hết hạn sau 5 phút.\n\nThân ái!";

            bool isSent = SendEmail(user.Username, subject, body);

            if (isSent)
                return Ok("Đã gửi mã OTP qua Email của bạn!");
            else
                return BadRequest("Lỗi khi gửi Email. Kiểm tra lại appsettings.json.");
        }

        // --- 4. XÁC THỰC OTP ---
        [HttpPost("verify-otp")]
        public async Task<IActionResult> VerifyOtp([FromBody] OtpRequest request)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == request.Username);
            if (user == null) return NotFound("Lỗi người dùng");

            if (user.OtpCode != request.OtpCode)
            {
                return BadRequest("Mã OTP không đúng!");
            }

            if (user.OtpExpiryTime < DateTime.Now)
            {
                return BadRequest("Mã OTP đã hết hạn!");
            }

            // OTP đúng -> Xóa OTP đi
            user.OtpCode = null;
            user.OtpExpiryTime = null;
            await _context.SaveChangesAsync();

            return Ok("Xác thực thành công!");
        }

        // --- 5. ĐỔI MẬT KHẨU MỚI (MỚI THÊM) ---
        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequest request)
        {
            // Tìm user theo Email (Username)
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == request.Email);

            if (user == null)
            {
                return BadRequest("Tài khoản không tồn tại!");
            }

            // Cập nhật mật khẩu mới
            user.Password = request.NewPassword;

            // Lưu vào Database
            await _context.SaveChangesAsync();

            return Ok(new { message = "Đổi mật khẩu thành công!" });
        }

        // --- HÀM GỬI EMAIL ---
        private bool SendEmail(string toEmail, string subject, string body)
        {
            try
            {
                var emailSettings = _configuration.GetSection("EmailSettings");
                var fromEmail = emailSettings["Mail"];
                var password = emailSettings["Password"];
                var host = emailSettings["Host"];
                var port = int.Parse(emailSettings["Port"]);

                var smtpClient = new SmtpClient(host)
                {
                    Port = port,
                    Credentials = new NetworkCredential(fromEmail, password),
                    EnableSsl = true,
                };

                var mailMessage = new MailMessage
                {
                    From = new MailAddress(fromEmail),
                    Subject = subject,
                    Body = body,
                    IsBodyHtml = false,
                };

                if (!toEmail.Contains("@")) return false; // Check sơ bộ

                mailMessage.To.Add(toEmail);
                smtpClient.Send(mailMessage);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Gửi mail thất bại: " + ex.Message);
                return false;
            }
        }
    }

    // --- CÁC CLASS DTO ---
    public class ForgotPassRequest
    {
        public string Username { get; set; } // Email
    }

    public class OtpRequest
    {
        public string Username { get; set; }
        public string OtpCode { get; set; }
    }

    // Class nhận dữ liệu đổi mật khẩu
    public class ResetPasswordRequest
    {
        public string Email { get; set; }
        public string NewPassword { get; set; }
    }
}