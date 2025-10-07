using Microsoft.EntityFrameworkCore;

public class HabitTrackerContext : DbContext
{
    public DbSet<Usuario> Usuarios {get; set; }
    public DbSet<Habito> Habitos {get; set; }
    public DbSet<RegistroDiario> RegistrosDiarios {get; set;}
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlite("Data Source=habitTracker.db");
    }
}