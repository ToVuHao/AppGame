using GameApp.API.Data;
using GameApp.API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GameApp.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GamesController : ControllerBase
    {
        private readonly GameDbContext _context;

        public GamesController(GameDbContext context)
        {
            _context = context;
        }

        // Lấy danh sách trạng thái Game
        [HttpGet]
        public async Task<IActionResult> GetGames()
        {
            return Ok(await _context.Games.ToListAsync());
        }

        // Cập nhật trạng thái (Chỉ Admin dùng)
        [HttpPut("{id}")]
        public async Task<IActionResult> ToggleGame(int id, [FromBody] bool isActive)
        {
            var game = await _context.Games.FindAsync(id);
            if (game == null) return NotFound();

            game.IsActive = isActive;
            await _context.SaveChangesAsync();
            return Ok(new { message = "Cập nhật thành công", isActive = game.IsActive });
        }
    }
}