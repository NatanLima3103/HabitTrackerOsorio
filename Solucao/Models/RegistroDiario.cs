using System;

public class RegistroDiario
{
    public int Id{get; set; } //chave primária
    public DateTime Data {get; set; } //A data em que o registro foi feito
    public bool Cumprido {get; set; } //Se o hábito for concluído TRUE, caso contrário, FALSE.
    public int HabitoId {get; set; } //Conexão do hábito com a chave estrangeira.
    public Habito Habito {get; set; }
}