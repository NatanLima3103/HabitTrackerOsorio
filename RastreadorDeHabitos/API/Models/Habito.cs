using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace API.Models;
public class Habito
{
    public int Id { get; set; }
    
    public string Nome {get; set; } = string.Empty;
    public string Descricao {get; set; } = string.Empty;
    public int UsuarioId {get; set; } //Chave estrangeira para conectar ao usuário.
    public Usuario? Usuario { get; set; }
    public DateTime CriadoEm { get; set; } = DateTime.Now;
    public ICollection<RegistroDiario> Registros { get; set; } = [];

    [NotMapped] //não vira uma coluna no banco de dados, apenas para uso da API
    public bool ConcluidoHoje { get; set; }
}