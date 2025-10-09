using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

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

    public Usuario CadastroUsuario(string nome, string email, string senha)
    {
        if (string.IsNullOrWhiteSpace(nome) || string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(senha))
        {
            throw new System.Exception("Nome, email e senha são obrigatórios.");
        }

        if (_context.Usuarios.Any(x => x.Email.ToLower() == email.ToLower()))
        {
            throw new System.Exception("Este e-mail já está cadastrado.");
        }

        string senhaHash = BCrypt.Net.BCrypt.HashPassword(senha);

        var novoUsuario = new Usuario
        {
            Nome = nome,
            Email = email,
            Senha = senhaHash
        };

        _context.Usuarios.Add(novoUsuario);
        _context.SaveChanges();

        return novoUsuario;
    }

    public string LoginUsuario(string email, string senha)
    {
        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(senha))
        {
            throw new System.Exception("Preencha todos os campos.");
        }

        Usuario? usuario = _context.Usuarios.FirstOrDefault(x => x.Email.ToLower() == email.ToLower()) ?? throw new System.Exception("Usuário não existe com email cadastrado.");

        bool senhaValida = BCrypt.Net.BCrypt.Verify(senha, usuario.Senha);

        if (!senhaValida)
        {
            throw new Exception("Senha incorreta!");
        }

        // Se a senha for válida, geramos o token JWT
        string token = GerarTokenJwt(usuario);
        
        return token;
    }

    // método para gerar Token JWT
    private string GerarTokenJwt(Usuario usuario)
    {
        // Obter a chave secreta do appsettings.json
        var jwtKey = _configuration["Jwt:Key"];
        if(string.IsNullOrEmpty(jwtKey))
        {
            throw new InvalidOperationException("A chave JWT não está configurada no appsettings.json");
        }
        
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        // 2. Definir as "Claims" do usuário (informações que estarão no token)
        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, usuario.Id.ToString()), // Id do usuário
            new Claim(JwtRegisteredClaimNames.Email, usuario.Email),
            new Claim(ClaimTypes.Name, usuario.Nome), // Nome do usuário
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()) // ID único para o token
        };

        // Criar o Token
        var token = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"],
            audience: _configuration["Jwt:Audience"],
            claims: claims,
            expires: DateTime.Now.AddHours(8), // Define o tempo de expiração do token
            signingCredentials: credentials);

        // Serializar o token para uma string
        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}