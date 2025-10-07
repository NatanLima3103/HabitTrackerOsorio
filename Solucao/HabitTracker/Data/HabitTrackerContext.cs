using Microsoft.EntityFrameworkCore;

public class HabitTrackerContext : DbContext
{
    public DbSet<Usuario> usuarios {get; set; }
    public DbSet<Habito> habitos {get; set; }
    public DbSet<RegistroDiario> RegistrosDiarios {get; set;}
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        //Define o uso de um banco de dados SQlite.
        optionsBuilder.UseSqlite("Data Source=habitTracker.db");
    }
}