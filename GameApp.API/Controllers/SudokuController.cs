using GameApp.API.Models;
using Microsoft.AspNetCore.Mvc;

namespace GameApp.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SudokuController : ControllerBase
    {
        [HttpGet("new-game")]
        public IActionResult GetNewGame()
        {
            // Cố định số ô trống là 40 (Mức độ cân bằng)
            // Không cần switch case phức tạp nữa
            int missingDigits = 40;

            SudokuHelper sudoku = new SudokuHelper(missingDigits);
            var gameData = sudoku.GenerateGame();

            return Ok(gameData);
        }
    }
}