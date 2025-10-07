using System.Collections.Generic;

public class Habito
{
    public int Id {get; set; }
    public string Nome {get; set; }
    public string Descricao {get; set; }
    public int UsuarioId {get; set; } //Chave estrangeira para conectar ao usuário.
    public Usuario Usuario {get; set; }

    public ICollection<RegistroDiario> Registros {get; set; }
}