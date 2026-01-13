using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GameApp.API.Models
{
    public class Friendship
    {
        [Key]
        public int Id { get; set; }

        public int SenderId { get; set; } // Người gửi lời mời
        public int ReceiverId { get; set; } // Người nhận lời mời

        // Trạng thái: "Pending" (Chờ), "Accepted" (Đã kết bạn), "Rejected" (Từ chối)
        public string Status { get; set; } = "Pending";

        public DateTime RequestDate { get; set; } = DateTime.Now;

        // Relationship (để dễ truy vấn tên)
        [ForeignKey("SenderId")]
        public User? Sender { get; set; }

        [ForeignKey("ReceiverId")]
        public User? Receiver { get; set; }
    }
}