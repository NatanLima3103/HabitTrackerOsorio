using System;

public class RegistroDiario
{
    public RegistroDiario()
    {
        Data = DateTime.Now; //pega a data atual
        Cumprido = false;
        Habito = new Habito(); //deixa uma lista de habitos vazia para criaçao de um usuario
    }

    public int Id { get; set; } //chave primária
    public DateTime Data {get; set; }//A data em que o registro foi feito
    public bool Cumprido {get; set; } //Se o hábito for concluído TRUE, caso contrário, FALSE.
    public int HabitoId {get; set; } //Conexão do hábito com a chave estrangeira.
    public Habito Habito {get; set; }
}