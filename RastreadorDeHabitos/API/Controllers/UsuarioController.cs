using Microsoft.AspNetCore.Mvc;
using API.Services; // namespace do UsuarioService
using System;

namespace API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UsuarioController : ControllerBase
    {
        private readonly UsuarioService _usuarioService;

        public UsuarioController(UsuarioService usuarioService)
        {
            _usuarioService = usuarioService;
        }

        // -------- CADASTRO DE USUÁRIO --------
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