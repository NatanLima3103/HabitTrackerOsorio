using Microsoft.EntityFrameworkCore;
using API.Services;
using API.UI;

var builder = WebApplication.CreateBuilder(args);

//desativa logs
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.SetMinimumLevel(LogLevel.Warning);
//Configuração do banco SQLite
builder.Services.AddDbContext<HabitTrackerContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection") ?? "Data Source=habittracker.db")
);

//Serviços
builder.Services.AddScoped<UsuarioService>();
builder.Services.AddScoped<HabitoService>();
builder.Services.AddScoped<StreakService>();

var app = builder.Build();

//Cria o banco e aplica migrations automaticamente
using var scope = app.Services.CreateScope();
var db = scope.ServiceProvider.GetRequiredService<HabitTrackerContext>();
db.Database.Migrate();

var usuarioService = scope.ServiceProvider.GetRequiredService<UsuarioService>();
var habitoService = scope.ServiceProvider.GetRequiredService<HabitoService>();
var streakService = scope.ServiceProvider.GetRequiredService<StreakService>();

TerminalUI.Iniciar(usuarioService, habitoService, streakService);
