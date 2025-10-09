using Microsoft.AspNetCore.Mvc;
using API.Services;
using System;

namespace API.Controllers
{
    /// <summary>
    /// Controller responsável pela gestão de usuários e autenticação no sistema.
    /// 
    /// Funcionalidades principais:
    /// - Cadastro de novos usuários no sistema
    /// - Autenticação (login) de usuários existentes
    /// - Validação de credenciais e dados de entrada
    /// 
    /// Endpoints disponíveis:
    /// POST /usuario/cadastrar - Cria uma nova conta de usuário
    /// POST /usuario/login - Realiza login e autentica usuário
    /// 
    /// Este controller trabalha em conjunto com UsuarioService para executar
    /// a lógica de negócio relacionada a usuários.
    /// </summary>
    [ApiController]
    [Route("[controller]")]
    public class UsuarioController : ControllerBase
    {
        private readonly UsuarioService _usuarioService;

        public UsuarioController(UsuarioService usuarioService)
        {
            _usuarioService = usuarioService;
        }

        //CADASTRO DE USUÁRIO
        [HttpPost("cadastrar")]
        public IActionResult Cadastrar([FromBody] Usuario usuario)
        {
            // verifica se o modelo passado é válido
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState); // retorna os erros detalhados
            }

            try
            {
                var novoUsuario = _usuarioService.CadastroUsuario(usuario.Nome, usuario.Email, usuario.Senha);
                return Ok(novoUsuario);
            }
            catch (Exception ex)
            {
                return BadRequest(new { mensagem = ex.Message });
            }
        }

        // -------- LOGIN DE USUÁRIO --------
        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginDto login)
        {
            if (string.IsNullOrWhiteSpace(login.Email) || string.IsNullOrWhiteSpace(login.Senha))
            {
                return BadRequest(new { mensagem = "Preencha todos os campos." });
            }

            try
            {
                var usuario = _usuarioService.LoginUsuario(login.Email, login.Senha);
                return Ok(usuario);
            }
            catch (Exception ex)
            {
                return BadRequest(new { mensagem = ex.Message });
            }
        }
    }
}

// DTO que representa os dados necessários para o login
public record LoginDto(string Email, string Senha);