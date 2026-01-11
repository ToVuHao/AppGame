using GameApp.API.Data;
using Microsoft.EntityFrameworkCore;
using GameApp.API.Hubs; // Import th? m?c ch?a ChatHub

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// 1. QUAN TR?NG: ??ng ký d?ch v? SignalR cho Chat
builder.Services.AddSignalR();

// K?t n?i SQL
builder.Services.AddDbContext<GameDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();

app.MapControllers();

// 2. QUAN TR?NG: ??nh ngh?a ???ng d?n cho "Tr?m phát sóng" Chat
app.MapHub<ChatHub>("/chatHub");

app.Run();