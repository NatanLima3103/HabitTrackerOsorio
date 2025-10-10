using System;

namespace API.Services;


public class HabitoService
{
    // 1. Injetar o contexto do banco de dados, assim como no UsuarioService
    private readonly HabitTrackerContext _context;

    // 2. O construtor recebe o DbContext via injeção de dependência
    public HabitoService(HabitTrackerContext context)
    {
        _context = context;
    }

    
    public Habito CriarHabito(string nome, string descricao, int usuarioId)
    {
        if (string.IsNullOrWhiteSpace(nome))
        {
            throw new System.Exception("O nome do hábito é obrigatório.");
        }

        
        var usuario = _context.Usuarios.Find(usuarioId);
        if (usuario == null)
        {
            throw new System.Exception("Usuário não encontrado.");
        }

        var novoHabito = new Habito
        {
            Nome = nome,
            Descricao = descricao,
            UsuarioId = usuarioId // Associa ao usuário logado
        };

        _context.Habitos.Add(novoHabito); 
        _context.SaveChanges();          

        return novoHabito;
    }

    
    public List<Habito> ListarHabitosDoUsuario(int usuarioId)
    {
        return _context.Habitos
                       .Where(h => h.UsuarioId == usuarioId)
                       .ToList();
    }
    public bool AtualizarHabito(int habitoId, string novoNome, string novaDescricao, int usuarioId)
    {
        var habitoExistente = _context.Habitos.FirstOrDefault(h => h.Id == habitoId && h.UsuarioId == usuarioId);

        if (habitoExistente == null)
        {
            return false;
        }

        if (!string.IsNullOrWhiteSpace(novoNome))
        {
            habitoExistente.Nome = novoNome;
        }

        if (!string.IsNullOrWhiteSpace(novaDescricao))
        {
            habitoExistente.Descricao = novaDescricao;
        }

        _context.SaveChanges(); 
        return true;
    }

    
    public bool ExcluirHabito(int habitoId, int usuarioId)
    {
        var habitoParaExcluir = _context.Habitos.FirstOrDefault(h => h.Id == habitoId && h.UsuarioId == usuarioId);

        if (habitoParaExcluir == null)
        {
            return false;
        }
        _context.Habitos.Remove(habitoParaExcluir); 
        _context.SaveChanges();                    

        return true;
    }

}
