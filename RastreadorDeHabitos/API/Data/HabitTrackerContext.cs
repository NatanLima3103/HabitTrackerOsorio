using Microsoft.EntityFrameworkCore;
namespace API.Models;

public class HabitTrackerContext : DbContext
{
    public HabitTrackerContext(DbContextOptions<HabitTrackerContext> options)
        : base(options)
    {
    }

    public DbSet<Usuario> Usuarios { get; set; }
    public DbSet<Habito> Habitos { get; set; }
    public DbSet<RegistroDiario> RegistrosDiarios { get; set; }
}
