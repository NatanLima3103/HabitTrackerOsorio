using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

public class Habito
{
    public int Id {get; set; }
    public string Nome {get; set; } = string.Empty;
    public string Descricao {get; set; } = string.Empty;
    public int UsuarioId {get; set; } //Chave estrangeira para conectar ao usuário.
    public Usuario? Usuario { get; set; }

    public int Streak { get; set; } = 0;
    public ICollection<RegistroDiario> Registros { get; set; } = [];
}