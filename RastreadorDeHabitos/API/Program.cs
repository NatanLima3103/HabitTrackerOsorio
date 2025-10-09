using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using API.Services;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<HabitTrackerContext>();
builder.Services.AddScoped<UsuarioService>();
// adiciona controller
builder.Services.AddControllers();
var app = builder.Build();
// adiciona controller
app.MapControllers();

app.Run();
