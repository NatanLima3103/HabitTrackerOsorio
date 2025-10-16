using System;

namespace API.Services;

public class StreakService
{

    private readonly HabitTrackerContext _context;

    public StreakService(HabitTrackerContext context)
    {
        _context = context;
    }


    public int IncrementarStreak(int habitoId)
    {
        // Busca o hábito no banco
        var habito = _context.Habitos.FirstOrDefault(h => h.Id == habitoId);
        if (habito == null)
            throw new Exception("Hábito não encontrado.");

        // Incrementa o streak
        habito.Streak++;

        // Salva no banco
        _context.SaveChanges();

        // Retorna o novo streak
        return habito.Streak;
    }

    public void MarcarHabitoComoConcluido(int usuarioId, int habitoId)
    {
        var habito = _context.Habitos.FirstOrDefault(h => h.Id == habitoId && h.UsuarioId == usuarioId);
        if (habito == null)
        {
            Console.WriteLine("Hábito não encontrado!\n");
            return;
        }

        var hoje = DateTime.Today;

        // busca se o o habito ja foi concluido hoje
        var registroHoje = _context.RegistrosDiarios
            .AsEnumerable()
            .FirstOrDefault(r =>
                r.HabitoId == habitoId &&
                r.Data.Date == hoje);

        // Já foi concluído hoje
        if (registroHoje != null && registroHoje.Cumprido)
        {
            Console.WriteLine($"\n⚠️ O hábito '{habito.Nome}' já foi concluído hoje!\n");
            return;
        }

        // Se ainda não existe registro hoje, cria
        if (registroHoje == null)
        {
            registroHoje = new RegistroDiario
            {
                HabitoId = habitoId,
                Data = DateTime.Now,
                Cumprido = true
            };
            _context.RegistrosDiarios.Add(registroHoje);
        }
        else
        {
            registroHoje.Cumprido = true;
            _context.RegistrosDiarios.Update(registroHoje);
        }

        _context.SaveChanges();

        Console.WriteLine($"\n✅ Hábito '{habito.Nome}' marcado como concluído!\n");

        // Verifica se todos foram concluídos
        VerificarConclusaoDiaria(usuarioId);
    }


    private void VerificarConclusaoDiaria(int usuarioId)
    {
        var hoje = DateTime.Today;

        // Todos os hábitos do usuário
        var habitosUsuario = _context.Habitos.Where(h => h.UsuarioId == usuarioId).ToList();

        // Hábitos concluídos hoje
        var concluidosHoje = _context.RegistrosDiarios
            .Where(r => r.Habito.UsuarioId == usuarioId && r.Data.Date == hoje && r.Cumprido)
            .Select(r => r.HabitoId)
            .Distinct()
            .ToList();

        if (concluidosHoje.Count == habitosUsuario.Count && habitosUsuario.Count > 0)
        {
            Console.WriteLine("🔥 Todos os hábitos foram concluídos hoje!");
            AtualizarStreakUsuario(usuarioId);
        }
        else
        {
            int faltam = habitosUsuario.Count - concluidosHoje.Count;
            Console.WriteLine($"Ainda faltam {faltam} hábito(s) para fechar o dia!\n");
        }
    }

    private void AtualizarStreakUsuario(int usuarioId)
    {
        var hoje = DateTime.Today;
        var ontem = hoje.AddDays(-1);

        var usuario = _context.Usuarios.FirstOrDefault(u => u.Id == usuarioId);
        if (usuario == null) return;

        // Verifica se ontem também foi um dia completo
        var habitosUsuario = _context.Habitos.Where(h => h.UsuarioId == usuarioId).ToList();

        var concluidosOntem = _context.RegistrosDiarios
            .Where(r => r.Habito.UsuarioId == usuarioId && r.Data.Date == ontem && r.Cumprido)
            .Select(r => r.HabitoId)
            .Distinct()
            .ToList();

        bool manteveSequencia = (concluidosOntem.Count == habitosUsuario.Count);

        // Incrementa ou reinicia
        usuario.Streak = manteveSequencia ? usuario.Streak + 1 : 1;

        _context.Usuarios.Update(usuario);
        _context.SaveChanges();

        Console.WriteLine(manteveSequencia
            ? $"🔥 Você manteve sua sequência! Streak atual: {usuario.Streak} dias!"
            : $"💀 Sequência reiniciada. Novo streak: {usuario.Streak}");
    }


    public void ExibirStreaksUsuario(int usuarioId)
    {
        var usuario = _context.Usuarios.FirstOrDefault(u => u.Id == usuarioId);
        if (usuario == null)
        {
            Console.WriteLine("⚠️ Usuário não encontrado!\n");
            return;
        }

        var hoje = DateTime.Today;
        var habitos = _context.Habitos.Where(h => h.UsuarioId == usuarioId).ToList();

        if (habitos.Count == 0)
        {
            Console.WriteLine("⚠️ Nenhum hábito cadastrado para este usuário.\n");
            return;
        }

        Console.WriteLine("\n====== Seus Streaks =====");
        Console.WriteLine($"Streak total do usuário: {usuario.Streak} dias 🔥\n");

        foreach (var h in habitos)
        {
            bool concluidoHoje = _context.RegistrosDiarios
                .Any(r => r.HabitoId == h.Id && r.Data.Date == hoje && r.Cumprido);

            string statusHoje = concluidoHoje ? "✅ Concluído hoje" : "❌ Não concluído hoje";
            Console.WriteLine($"Hábito: {h.Nome} | Streak: {h.Streak} dias | {statusHoje}");
        }

        Console.WriteLine("==========================\n");
    }

}
