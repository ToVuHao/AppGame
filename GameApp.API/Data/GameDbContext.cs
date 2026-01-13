using GameApp.API.Models;
using Microsoft.EntityFrameworkCore;

namespace GameApp.API.Data
{
    public class GameDbContext : DbContext
    {
        public GameDbContext(DbContextOptions<GameDbContext> options) : base(options) { }

        // --- CÁC BẢNG TRONG DB ---
        public DbSet<User> Users { get; set; }
        public DbSet<ChatMessage> ChatMessages { get; set; }
        public DbSet<Friendship> Friendships { get; set; }
        public DbSet<GameInfo> Games { get; set; } // <--- MỚI: Thêm bảng này để quản lý Game

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // 1. Cấu hình bảng Kết Bạn (Friendship)
            modelBuilder.Entity<Friendship>()
                .HasOne(f => f.Sender)
                .WithMany()
                .HasForeignKey(f => f.SenderId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Friendship>()
                .HasOne(f => f.Receiver)
                .WithMany()
                .HasForeignKey(f => f.ReceiverId)
                .OnDelete(DeleteBehavior.Restrict);

            // 2. Seed Data (Tạo sẵn danh sách Game khi khởi tạo DB)
            modelBuilder.Entity<GameInfo>().HasData(
                new GameInfo { Id = 1, Name = "Cờ tướng", IsActive = true },
                new GameInfo { Id = 2, Name = "Sudoku", IsActive = true },
                new GameInfo { Id = 3, Name = "Xếp hình", IsActive = true },
                new GameInfo { Id = 4, Name = "Caro vs Máy", IsActive = true }
            );
        }
    }
}