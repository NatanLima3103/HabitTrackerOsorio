using API.Models;
namespace API.Services
{
    public class LoginInputModel
    {
        public string Email { get; set; } = string.Empty;
        public string Senha { get; set; } = string.Empty;
    }


    public class UsuarioService
    {
        private readonly HabitTrackerContext _context;

        public UsuarioService(HabitTrackerContext context)
        {
            _context = context;
        }
        public Usuario? Autenticar(LoginInputModel login)
        {
            // Encontra o usuário pelo Email
            var usuario = _context.Usuarios
                .FirstOrDefault(u => u.Email.ToLower() == login.Email.ToLower());

            // Se o usuário não existe, falha.
            if (usuario == null)
            {
                return null;
            }

            bool senhaValida = BCrypt.Net.BCrypt.Verify(login.Senha, usuario.Senha);
            if (!senhaValida)
            {
                return null;
            }
            // Retorna o usuário encontrado.
            return usuario;
        }
    }
}