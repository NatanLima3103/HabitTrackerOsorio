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
        if (_context.Usuarios.Any(x => x.Email.ToLower() == email.ToLower()))
        {
            throw new System.Exception("Este e-mail já está cadastrado.");
        }

        // gera hash da senha para não ficar exposta
        string senhaHash = BCrypt.Net.BCrypt.HashPassword(senha);

        // cria novo usuário
        var novoUsuario = new Usuario
        {
            Nome = nome,
            Email = email,
            Senha = senhaHash
        };

        // adiciona o novo usuário ao DbContext e salva as alterações no banco de dados
        _context.Usuarios.Add(novoUsuario);
        _context.SaveChanges();

        return novoUsuario;
    }
    
    public Usuario LoginUsuario(string email, string senha)
    {   
        //verifica se todos os campos estao preenchidos
        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(senha))
        {
            throw new System.Exception("Preencha todos os campos.");
        }

        //procura o usuario no banco, caso nao exista lança exceção
        Usuario? usuario = _context.Usuarios.FirstOrDefault(x => x.Email.ToLower() == email.ToLower()) ?? throw new System.Exception("Usuário não existe com email cadastrado.");

        //verifica senha do usuario
        bool senhaValida = BCrypt.Net.BCrypt.Verify(senha, usuario.Senha);

        if (!senhaValida)
        {
            throw new Exception("Senha incorreta!");
        }

        usuario.Senha = null;
        return usuario;        
    }
}
