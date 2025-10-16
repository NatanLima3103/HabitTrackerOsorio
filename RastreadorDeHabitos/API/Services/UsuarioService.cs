namespace API.Services;

public class UsuarioService
{
    private readonly HabitTrackerContext _context;
    private readonly IConfiguration _configuration; // Adicionado para ler as configurações

    // Construtor modificado para receber IConfiguration
    public UsuarioService(HabitTrackerContext context, IConfiguration configuration)
    {
        _context = context;
        _configuration = configuration;
    }

    public void CadastroUsuario()
    {
        Console.WriteLine("\n====================");
        Console.WriteLine("      CADASTRO          ");
        Console.WriteLine("====================\n");
        Console.Write("Nome: ");
        string nome = Console.ReadLine()!;
        Console.Write("Email: ");
        string email = Console.ReadLine()!;
        Console.Write("Senha: ");
        string senha = Console.ReadLine()!;

        try
        {
            // Verifica se os campos não estão vazios
            if (string.IsNullOrWhiteSpace(nome) || string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(senha))
            {
                Console.WriteLine("Nome, email e senha são obrigatórios.\n");
                return;
            }

            // Verifica se o email já existe
            if (_context.Usuarios.Any(x => x.Email.ToLower() == email.ToLower()))
            {
                Console.WriteLine("Este e-mail já está cadastrado.\n");
                return;
            }

            // Cria hash da senha
            string senhaHash = BCrypt.Net.BCrypt.HashPassword(senha);

            var novoUsuario = new Usuario
            {
                Nome = nome,
                Email = email,
                Senha = senhaHash
            };


            _context.Usuarios.Add(novoUsuario);
            _context.SaveChanges();

            Console.WriteLine($"\n✅ Usuário {novoUsuario.Nome} cadastrado com sucesso! 🎉\n");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro no cadastro: {ex.Message}\n");
        }
    }


    public Usuario? LoginUsuario()
    {

        Console.WriteLine("\n====================");
        Console.WriteLine("      LOGIN          ");
        Console.WriteLine("====================\n");

        Console.Write("Email: ");
        string email = Console.ReadLine()!;
        Console.Write("Senha: ");
        string senha = Console.ReadLine()!;

        try
        {
            var usuario = _context.Usuarios
                .FirstOrDefault(x => x.Email.ToLower() == email.ToLower());

            if (usuario == null)
            {
                Console.WriteLine("Usuário não encontrado.\n");
                return null;
            }

            bool senhaValida = BCrypt.Net.BCrypt.Verify(senha, usuario.Senha);
            if (!senhaValida)
            {
                Console.WriteLine("Senha incorreta!\n");
                return null;
            }
            
            Console.WriteLine("\n👋 Bem-vindo ao Rastreador de Hábitos!\n");
            Console.WriteLine($"\nBem-vindo, {usuario.Nome}!");
            Console.WriteLine($"🔥 Streak atual: {usuario.Streak} dias!\n");

            return usuario;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro no login: {ex.Message}\n");
            return null;
        }
    }
}