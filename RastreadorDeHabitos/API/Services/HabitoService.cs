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
        if (listaHabitos is null)
        {
            throw new SystemException("Usuário não possui hábitos cadastrador");
        }

        Console.WriteLine("\n=== Seus Hábitos ===");
        foreach (var h in listaHabitos)
        {
            Console.WriteLine($"ID: {h.Id} | Nome: {h.Nome} | Descrição: {h.Descricao}");
        }
        Console.WriteLine();
        return listaHabitos;
    }

    public Habito? CriarHabito(int usuarioId)
    {
        Console.Write("Nome do hábito: ");
        string nomeHabito = Console.ReadLine()!;
        Console.Write("Descrição: ");
        string descricaoHabito = Console.ReadLine()!;
        if (string.IsNullOrWhiteSpace(nomeHabito) || string.IsNullOrWhiteSpace(descricaoHabito))
        {
            Console.WriteLine("Todos os campos devem ser preenchidos!");
            return null;
        }
        var habito = new Habito { Nome = nomeHabito, Descricao = descricaoHabito, UsuarioId = usuarioId };
        _context.Habitos.Add(habito);
        _context.SaveChanges();
        Console.WriteLine("Hábito criado com sucesso!\n");
        return habito;
    }

    public bool ExcluirHabito(int usuarioId)
    {
        Console.Write("ID do hábito a deletar: (Para ver todos os hábitos digite 0)");
        if (int.TryParse(Console.ReadLine(), out int idHabito))
        {   
            if(idHabito == 0)
            {
                ListarHabitosDoUsuario(usuarioId);
            }
            var habito = _context.Habitos.FirstOrDefault(h => h.Id == idHabito && h.UsuarioId == usuarioId);
            if (habito == null)
            {
                throw new SystemException("Habito com o id: " + idHabito + " não existe!");
            }
            Console.WriteLine("");
            Console.WriteLine("Hábito ID: " + habito.Id + " deletado!");
            Console.WriteLine("");
            _context.Habitos.Remove(habito);
            _context.SaveChanges();
            return true;
        }
        else
        {
            Console.WriteLine("ID inválido!\n");
            return false;
        }
    }

    public Habito? AtualizarHabito(int usuarioId)
    {
        while (true) // loop até atualizar com sucesso ou cancelar
        {
            Console.Write("ID do hábito a atualizar: (Para ver todos os hábitos digite 0 / Para cancelar digite -1) ");
            string? input = Console.ReadLine();

            if (!int.TryParse(input, out int habitoId))
            {
                Console.WriteLine("ID inválido!\n");
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

            // pede o novo nome e descrição
            Console.Write("Novo nome do hábito: ");
            var nome = Console.ReadLine();
            Console.Write("Nova descrição do hábito: ");
            var descricao = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(nome) || string.IsNullOrWhiteSpace(descricao))
            {
                Console.WriteLine("Todos os campos devem ser preenchidos!\n");
                continue; // volta para pedir ID novamente
            }

            habito.Nome = nome!;
            habito.Descricao = descricao!;
            _context.Habitos.Update(habito);
            _context.SaveChanges();

            Console.WriteLine("Hábito atualizado com sucesso!");
            Console.WriteLine($"ID: {habito.Id} | Nome: {habito.Nome} | Descrição: {habito.Descricao}\n");

            return habito; // sai após atualização
        }
    }
    private int CalcularStreak(int habitoId) {
    // Busca todos os registros concluídos do hábito, em ordem decrescente de data
    var registros = _context.RegistrosDiarios
        .Where(r => r.HabitoId == habitoId && r.Cumprido)
        .OrderByDescending(r => r.Data)
        .ToList();

    if (registros.Count == 0)
        return 0; // Nenhum registro concluído → streak = 0

    int streak = 0;
    DateTime diaEsperado = DateTime.Today;

    foreach (var registro in registros)
    {
        // Se o registro é do dia esperado, incrementa streak
        if (registro.Data.Date == diaEsperado)
        {
            streak++;
            diaEsperado = diaEsperado.AddDays(-1); // espera o dia anterior na próxima iteração
        }
        else if (registro.Data.Date < diaEsperado)
        {
            // Quebrou a sequência → para de contar
            break;
        }
        // Se for maior que o esperado (futuro), ignora — só conta consecutivos para trás
    }

    return streak;
}
    public void MarcarHabitoComoConcluido(int usuarioId, int habitoId)
    {
        var habito = _context.Habitos.FirstOrDefault(h => h.Id == habitoId && h.UsuarioId == usuarioId);
        var hoje = DateTime.Today;
        var registroHoje = _context.RegistrosDiarios
            .FirstOrDefault(r => r.HabitoId == habitoId && r.Data == hoje);

        // if (registroHoje == null)
        // {
        //     // Cria novo registro diário
        //     registroHoje = new RegistroDiario
        //     {
        //         Data = hoje,
        //         Cumprido = true,
        //         HabitoId = habitoId,
        //     };

        //     _context.RegistrosDiarios.Add(registroHoje);
        //     _context.SaveChanges();

        //     Console.WriteLine($"✅ Hábito '{habito.Nome}' marcado como concluído hoje ({hoje:dd/MM/yyyy})!");
        // }
        // else        
        if (habito == null)
        {
            Console.WriteLine("Hábito não encontrado!\n");
            return;
        }      
        else
        {
            // Já foi concluído hoje
            CalcularStreak(habitoId);
            Console.WriteLine($"\n⚠️ Você já marcou o hábito '{habito.Nome}' como concluído hoje!");
        }

        // Calcula e exibe o streak atual
        int streak = CalcularStreak(habitoId);
        Console.WriteLine($"🔥 Streak atual: {streak} dia(s) seguidos!\n");
    }
}