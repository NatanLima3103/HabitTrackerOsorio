public class RegistroDiario
{
    public RegistroDiario()
    {
        Data = DateTime.Now;
        Cumprido = false;
    }

    public int Id { get; set; }
    public DateTime Data { get; set; }
    public bool Cumprido { get; set; }
    public int HabitoId { get; set; }  // chave estrangeira
    public Habito Habito { get; set; } = null!;
}
