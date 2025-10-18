public class HabitoService
{
    private readonly HabitTrackerContext _context;

    public HabitoService(HabitTrackerContext context)
    {
        _context = context;
    }

    public List<Habito> ListarHabitosDoUsuario(int usuarioId)
    {
        var listaHabitos = _context.Habitos.Where(h => h.UsuarioId == usuarioId).ToList();
        if (listaHabitos.Count == 0)
        {
            Console.WriteLine("\nVocê ainda não possui hábitos cadastrados.");
            Console.WriteLine("Use a opção 2 para criar seu primeiro hábito.\n");
            return listaHabitos;
        }

        
        var hoje = DateTime.Today;       

        Console.WriteLine("\n====== Seus Hábitos =====");
        foreach (var h in listaHabitos)
        {
            // Verifica se existe um registro concluído hoje
            bool concluidoHoje = _context.RegistrosDiarios
            .Any(r => r.HabitoId == h.Id && r.Data.Date == hoje && r.Cumprido);
            string status = concluidoHoje ? "✅ Concluído hoje" : "❌ Não concluído hoje";
            
            Console.WriteLine($"ID: {h.Id} | Nome: {h.Nome} | Descrição: {h.Descricao} | Status: {status}");
        }
        Console.WriteLine("==========================\n");
        return listaHabitos;
    }

    public Habito? CriarHabito(int usuarioId)
    {
        Console.WriteLine("\n===== Criar Hábito =====");
        Console.Write("Nome do hábito: ");
        string nomeHabito = Console.ReadLine()!;
        Console.Write("Descrição: ");
        string descricaoHabito = Console.ReadLine()!;
        Console.WriteLine("==========================\n");

        if (string.IsNullOrWhiteSpace(nomeHabito) || string.IsNullOrWhiteSpace(descricaoHabito))
        {
            Console.WriteLine("Todos os campos devem ser preenchidos!");
            return null;
        }
        var habito = new Habito { Nome = nomeHabito, Descricao = descricaoHabito, UsuarioId = usuarioId };
        _context.Habitos.Add(habito);
        _context.SaveChanges();
        Console.WriteLine("✅ Hábito criado com sucesso!\n");
        return habito;
    }

    public void ExcluirHabito(int usuarioId)
    {

        Console.WriteLine("\n===== Excluir Hábito =====");
        while (true)
        {
            Console.Write("ID do hábito a deletar (Para ver todos os hábitos digite 0 / Para cancelar digite -1): ");
            string? input = Console.ReadLine();

            if (!int.TryParse(input, out int idHabito))
            {
                Console.WriteLine("❌ ID inválido! Tente novamente.\n");
                continue; // volta para pedir novamente
            }

            if (idHabito == -1) // opção de cancelar
            {
                Console.WriteLine("🛑 Exclusão cancelada.\n");
                break;
            }

            if (idHabito == 0) // lista hábitos
            {
                ListarHabitosDoUsuario(usuarioId);
                continue; // volta para pedir novamente
            }

            var habito = _context.Habitos.FirstOrDefault(h => h.Id == idHabito && h.UsuarioId == usuarioId);
            if (habito == null)
            {
                Console.WriteLine($"⚠️ Hábito com o id {idHabito} não existe!\n");
                continue; // volta para pedir novamente
            }

            try
            {
                _context.Habitos.Remove(habito);
                _context.SaveChanges();

                Console.WriteLine($"✅ Hábito ID: {habito.Id} deletado com sucesso!\n");
                break; // saiu do loop após deletar
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Ocorreu um erro ao deletar o hábito: {ex.Message}\n");
                break; // sai do loop se ocorrer erro grave
            }
        }
    }

    public Habito? AtualizarHabito(int usuarioId)
    {   
        Console.WriteLine("\n===== Atualizar Hábito =====");        
        while (true) // loop até atualizar com sucesso ou cancelar
        {
            Console.Write("ID do hábito a atualizar: (Para ver todos os hábitos digite 0 / Para cancelar digite -1) ");
            string? input = Console.ReadLine();

            if (!int.TryParse(input, out int habitoId))
            {
                Console.WriteLine("❌ ID inválido!\n");
                continue; // volta para pedir o ID novamente
            }

            if (habitoId == -1) // opção de cancelar
            {
                Console.WriteLine("Atualização cancelada.\n");
                return null;
            }

            if (habitoId == 0) // apenas lista os hábitos
            {
                ListarHabitosDoUsuario(usuarioId);
                continue; // volta para pedir o ID novamente
            }

            var habito = _context.Habitos.FirstOrDefault(h => h.Id == habitoId && h.UsuarioId == usuarioId);
            if (habito == null)
            {
                Console.WriteLine($"Hábito com o id {habitoId} não existe!\n");
                continue; // volta para pedir o ID novamente
            }

            // pede o novo nome e descrição, caso vazia mantem valor antigo
            Console.Write($"Novo nome do hábito (atual: {habito.Nome}): ");
            var nome = Console.ReadLine();
            Console.Write($"Nova descrição (atual: {habito.Descricao}): ");
            var descricao = Console.ReadLine();

            habito.Nome = string.IsNullOrWhiteSpace(nome) ? habito.Nome : nome;
            habito.Descricao = string.IsNullOrWhiteSpace(descricao) ? habito.Descricao : descricao;

            _context.Habitos.Update(habito);
            _context.SaveChanges();

            Console.WriteLine("Hábito atualizado com sucesso!");
            Console.WriteLine($"ID: {habito.Id} | Nome: {habito.Nome} | Descrição: {habito.Descricao}\n");

            return habito; // sai após atualização
        }
    }
    
}