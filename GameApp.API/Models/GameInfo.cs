using System.ComponentModel.DataAnnotations;

namespace GameApp.API.Models
{
    public class GameInfo
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; } // Tên game (Sudoku, Caro...)
        public bool IsActive { get; set; } = true; // True = Hoạt động, False = Bảo trì
    }
}