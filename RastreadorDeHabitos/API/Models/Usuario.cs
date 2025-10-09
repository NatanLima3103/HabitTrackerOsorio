using System.ComponentModel.DataAnnotations;

public class Usuario
{

    public Usuario()
    {
        Habitos = [];//deixa uma lista de habitos vazia para criaçao de um usuario
    }

    public int Id { get; set; }

    [Required]
    public string Nome { get; set; } = string.Empty;

    [Required, EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required]
    public string Senha { get; set; } = string.Empty;

    public ICollection<Habito> Habitos { get; set; }
}
