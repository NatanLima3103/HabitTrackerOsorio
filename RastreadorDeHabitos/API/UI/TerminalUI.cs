using API.Services;

namespace API.UI;

public static class TerminalUI
{
    public static void Iniciar(UsuarioService usuarioService, HabitoService habitoService)
    {
        Console.WriteLine("=== RASTREADOR DE HÁBITOS ===\n");

        Usuario? usuarioLogado = null;
        int usuarioId = 0;
        string usuarioNome = "";

        // Escolha inicial: Cadastro ou Login 
        while (usuarioLogado == null)
        {
            Console.WriteLine("Escolha uma opção:");
            Console.WriteLine("1 - Cadastrar");
            Console.WriteLine("2 - Login");
            Console.WriteLine("0 - Sair");
            Console.Write("Opção: ");
            string escolha = Console.ReadLine()!;
            Console.WriteLine();

            if (escolha == "1") { usuarioService.CadastroUsuario(); }
            else if (escolha == "2") 
            {
                usuarioLogado = usuarioService.LoginUsuario();
                if (usuarioLogado != null)
                {
                    usuarioId = usuarioLogado.Id;
                    usuarioNome = usuarioLogado.Nome;
                } 
            }
            else if (escolha == "0") { return; }
            else { Console.WriteLine("Opção inválida!\n"); }
            
            
        }

        // --- Menu principal ---
        while (true)
        {
            Console.WriteLine("Escolha uma opção:");
            Console.WriteLine("1 - Listar hábitos");
            Console.WriteLine("2 - Criar hábito");
            Console.WriteLine("3 - Atualizar hábito");
            Console.WriteLine("4 - Deletar hábito");
            Console.WriteLine("5 - Marcar hábito como concluído");
            Console.WriteLine("0 - Sair");
            Console.Write("Opção: ");
            string opcao = Console.ReadLine()!;
            Console.WriteLine();

            switch (opcao)
            {
                case "1":
                    {habitoService.ListarHabitosDoUsuario(usuarioId);
                    break;}
                case "2":
                    {habitoService.CriarHabito(usuarioId);
                    break;}
                case "3":
                    {habitoService.AtualizarHabito(usuarioId);
                    break;
                    }
                case "4":
                    {habitoService.ExcluirHabito(usuarioId);
                    break;}
                case "5":
                    {
                        habitoService.ListarHabitosDoUsuario(usuarioId);

                        Console.Write("\nDigite o ID do hábito que deseja marcar como concluído: ");
                        if (int.TryParse(Console.ReadLine(), out int habitoId))
                        {
                            habitoService.MarcarHabitoComoConcluido(usuarioId, habitoId);
                        }
                        else
                        {
                            Console.WriteLine("ID inválido. Tente novamente.");
                        }
                        break;
                    }
                case "0":
                    {return;}
                default:
                    Console.WriteLine("Opção inválida!\n");
                    break;
            }
        }
    }
}
