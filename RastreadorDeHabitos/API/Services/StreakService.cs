using API.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;

namespace API.Services
{
    public class StreakService
    {
        private readonly HabitTrackerContext _context;

        public StreakService(HabitTrackerContext context)
        {
            _context = context;
        }

        public (string, int) VerificarConclusaoDiaria(int usuarioId)
        {
            var hoje = DateTime.Today;

            var habitosUsuario = _context.Habitos.Where(h => h.UsuarioId == usuarioId).ToList();
            if (habitosUsuario.Count == 0)
                return (string.Empty, 0);

            var concluidosHoje = _context.RegistrosDiarios
                .Where(r => r.Habito.UsuarioId == usuarioId && r.Data.Date == hoje && r.Cumprido)
                .Select(r => r.HabitoId)
                .Distinct()
                .ToList();

            if (concluidosHoje.Count == habitosUsuario.Count)
            {
                // Chama a função privada desta classe
                return AtualizarStreakUsuario(usuarioId);
            }
            else
            {
                int faltam = habitosUsuario.Count - concluidosHoje.Count;
                if (faltam > 0)
                    return ($"Ainda faltam {faltam} hábito{(faltam > 1 ? "s" : "")} para fechar o dia!", 0);
            }
            return (string.Empty, 0);
        }

        private (string, int) AtualizarStreakUsuario(int usuarioId)
        {
            var hoje = DateTime.Today;
            var ontem = hoje.AddDays(-1);

            var usuario = _context.Usuarios.FirstOrDefault(u => u.Id == usuarioId);
            if (usuario == null) return ("Usuário não encontrado.", 0);

            var habitosUsuarioOntem = _context.Habitos
                .Where(h => h.UsuarioId == usuarioId && h.CriadoEm.Date <= ontem)
                .ToList();

            var concluidosOntem = _context.RegistrosDiarios
                .Where(r => r.Habito.UsuarioId == usuarioId &&
                            r.Data.Date == ontem &&
                            r.Cumprido)
                .Select(r => r.HabitoId)
                .Distinct()
                .ToList();

            bool manteveSequencia = false;
            if (habitosUsuarioOntem.Count > 0)
            {
                manteveSequencia = concluidosOntem.Count == habitosUsuarioOntem.Count;
            }
            else
            {
                manteveSequencia = true;
            }

            if (usuario.UltimaAtualizacaoStreak.Date == hoje)
            {
                return ($"Streak já atualizado hoje: {usuario.Streak} dias.", usuario.Streak);
            }

            if (manteveSequencia)
            {
                usuario.Streak++;
            }
            else
            {
                usuario.Streak = 1;
            }

            usuario.UltimaAtualizacaoStreak = hoje;
            _context.Usuarios.Update(usuario);
            _context.SaveChanges();

            string mensagem = manteveSequencia
                ? $"Você manteve sua sequência! Streak atual: {usuario.Streak} dias!"
                : $"Streak iniciado ou atualizado. Streak atual: {usuario.Streak}";

            return (mensagem, usuario.Streak);
        }

        public void VerificarSePerdeuStreak(int usuarioId)
        {
            var hoje = DateTime.Today;
            var ontem = hoje.AddDays(-1);

            var usuario = _context.Usuarios.FirstOrDefault(u => u.Id == usuarioId);
            if (usuario == null) return;

            if (usuario.UltimaAtualizacaoStreak.Date == hoje)
                return;

            var habitosUsuario = _context.Habitos
                .Where(h => h.UsuarioId == usuarioId && h.CriadoEm.Date <= ontem)
                .ToList();

            if (habitosUsuario.Count == 0)
            {
                usuario.UltimaAtualizacaoStreak = hoje;
                _context.Usuarios.Update(usuario);
                _context.SaveChanges();
                return;
            }

            var concluidosOntem = _context.RegistrosDiarios
                .Where(r => r.Habito.UsuarioId == usuarioId &&
                            r.Data.Date == ontem &&
                            r.Cumprido)
                .Select(r => r.HabitoId)
                .Distinct()
                .ToList();

            if (concluidosOntem.Count != habitosUsuario.Count)
            {
                usuario.Streak = 0;
            }

            usuario.UltimaAtualizacaoStreak = hoje;
            _context.Usuarios.Update(usuario);
            _context.SaveChanges();
        }
    }
}