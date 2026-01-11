using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace GameApp.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LeaderboardController : ControllerBase
    {
        // Model dữ liệu trả về
        public class ScoreItem
        {
            public int Rank { get; set; }
            public string PlayerName { get; set; }
            public string Score { get; set; } // Để string để hiển thị linh hoạt (Điểm hoặc Thời gian)
            public string Date { get; set; }
        }

        [HttpGet]
        public IActionResult GetLeaderboard(string gameName)
        {
            List<ScoreItem> ranking = new List<ScoreItem>();

            // Tạo dữ liệu giả lập tùy theo game
            switch (gameName)
            {
                case "Sudoku":
                    ranking = new List<ScoreItem>
                    {
                        new ScoreItem { Rank = 1, PlayerName = "Admin Vip", Score = "02:15", Date = "01/01" },
                        new ScoreItem { Rank = 2, PlayerName = "Huy", Score = "03:45", Date = "02/01" },
                        new ScoreItem { Rank = 3, PlayerName = "Phuc", Score = "05:10", Date = "02/01" },
                        new ScoreItem { Rank = 4, PlayerName = "User 1", Score = "10:00", Date = "03/01" },
                    };
                    break;

                case "Cờ tướng":
                    ranking = new List<ScoreItem>
                    {
                        new ScoreItem { Rank = 1, PlayerName = "Kỳ Thánh", Score = "100 Trận thắng", Date = "01/01" },
                        new ScoreItem { Rank = 2, PlayerName = "Admin", Score = "80 Trận thắng", Date = "02/01" },
                        new ScoreItem { Rank = 3, PlayerName = "Người mới", Score = "5 Trận thắng", Date = "04/01" },
                    };
                    break;

                case "Caro vs Máy":
                    ranking = new List<ScoreItem>
                    {
                        new ScoreItem { Rank = 1, PlayerName = "AI Master", Score = "Bất bại", Date = "Forever" },
                        new ScoreItem { Rank = 2, PlayerName = "Bạn", Score = "Thắng 2 ván", Date = "Hôm nay" },
                    };
                    break;

                default: // Các game khác (Xếp hình, v.v.)
                    ranking = new List<ScoreItem>
                    {
                        new ScoreItem { Rank = 1, PlayerName = "Tester", Score = "1000 điểm", Date = "01/01" },
                        new ScoreItem { Rank = 2, PlayerName = "Dev", Score = "800 điểm", Date = "01/01" },
                    };
                    break;
            }

            return Ok(ranking);
        }
    }
}