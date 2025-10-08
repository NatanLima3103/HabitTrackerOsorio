using System;
namespace API.Services;

public class UsuarioService
{
    private readonly HabitTrackerContext _context;

    public UsuarioService(HabitTrackerContext context)
    {
        _context = context;
    }

    public Usuario CadastroUsuario(string nome, string email, string senha)
    {
        //verifica se nao estao vazios
        if (string.IsNullOrWhiteSpace(nome) || string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(senha))
        {
            throw new System.Exception("Nome, email e senha são obrigatórios.");
        }

        // verifica se o e-mail já existe no banco
        if (_context.Usuarios.Any(u => u.Email.ToLower() == email.ToLower()))
        {
            throw new System.Exception("Este e-mail já está cadastrado.");
        }

        // cria novo usuário
        var novoUsuario = new Usuario
        {
            Nome = nome,
            Email = email,
            Senha = senha 
        };

        // adiciona o novo usuário ao DbContext e salva as alterações no banco de dados
        _context.Usuarios.Add(novoUsuario);
        _context.SaveChanges();
        return novoUsuario;
    }
}
