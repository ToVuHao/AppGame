namespace GameApp.API.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Password { get; set; } // Trong thực tế nên dùng hash
        public string FullName { get; set; }

        public string Role { get; set; } = "user";

        public string? OtpCode { get; set; } // Mã OTP (ví dụ: "123456")
        public DateTime? OtpExpiryTime { get; set; } // Thời gian hết hạn
    }
}