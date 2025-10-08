using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using API.Services;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<HabitTrackerContext>();
builder.Services.AddScoped<UsuarioService>();
var app = builder.Build();


app.Run();