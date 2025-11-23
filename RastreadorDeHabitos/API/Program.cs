using Microsoft.EntityFrameworkCore;
using API.Services; 
using Microsoft.AspNetCore.Mvc;
using API.Models; 



var builder = WebApplication.CreateBuilder(args);



//Configuração do banco SQLite
builder.Services.AddDbContext<HabitTrackerContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection") ?? "Data Source=habittracker.db")
);

//Configuração do CORS 
builder.Services.AddCors(options =>
    options.AddPolicy("Acesso Total",
        configs => configs
            .AllowAnyOrigin()
  
           .AllowAnyHeader()
            .AllowAnyMethod())
);

builder.Services.AddScoped<UsuarioService>();
builder.Services.AddScoped<StreakService>();

var app = builder.Build();

//Cria o banco e aplica migrations automaticamente
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<HabitTrackerContext>();
    db.Database.Migrate();
}

//SERVIÇOS DE HÁBITOS

// GET: /api/habitos/usuario/{usuarioId}
app.MapGet("/api/habitos/usuario/{usuarioId}",
    ([FromServices] HabitTrackerContext db, int usuarioId) =>
    {
        //Lógica de busca
        var listaHabitos = db.Habitos
                             .Where(h => h.UsuarioId == usuarioId)
    
                          .ToList();

        var hoje = DateTime.Today;

        foreach (var h in listaHabitos)
        {
            //Marca se o hábito foi concluído hoje
            h.ConcluidoHoje = db.RegistrosDiarios
                                .Any(r => r.HabitoId 
 == h.Id && r.Data.Date == hoje && r.Cumprido);
        }

        return Results.Ok(listaHabitos);
    });

// GET: /api/habitos/buscar/{habitoId}
app.MapGet("/api/habitos/buscar/{habitoId}",
    ([FromServices] HabitTrackerContext db, int habitoId) =>
    {
        //Lógica de busca
        var habito = db.Habitos.FirstOrDefault(h => h.Id == habitoId);

        if (habito == null)
        {
            return Results.NotFound();
        }

        return Results.Ok(habito);
    });
// POST: /api/habitos/cadastrar
app.MapPost("/api/habitos/cadastrar",
    ([FromServices] HabitTrackerContext db, [FromBody] Habito habito) =>
    {
        if (string.IsNullOrWhiteSpace(habito.Nome) || string.IsNullOrWhiteSpace(habito.Descricao))
        {
            return Results.BadRequest("Nome e Descrição são campos obrigatórios.");
        }
        //Cria o objeto de hábito
        var novoHabito = new Habito
        {
            Nome = habito.Nome,
            Descricao = habito.Descricao,
  
           UsuarioId = habito.UsuarioId
        };

        //Salvar no banco
        db.Habitos.Add(novoHabito);
        db.SaveChanges();
        return Results.Created($"{novoHabito.Id}", novoHabito);
    });

//DELETE:  /api/habitos/deletar/{id}
app.MapDelete("/api/habitos/deletar/{id}",
    ([FromRoute] int id, [FromServices] HabitTrackerContext ctx) =>
    {
        Habito? resultado = ctx.Habitos.Find(id);
        if (resultado == null)
            return Results.NotFound("Hábito não encontrado.");
        ctx.Habitos.Remove(resultado);
 
        ctx.SaveChanges();
        return Results.Ok(resultado);
    });

//PATCH:  /api/habitos/alterar/{id}
app.MapPatch("/api/habitos/alterar/{id}",
    ([FromRoute] int id, [FromBody] Habito habitoAtualizado, [FromServices] HabitTrackerContext ctx) =>
    {
        Habito? habitoExistente = ctx.Habitos.Find(id);
        if (habitoExistente == null)
            return Results.NotFound("Hábito não encontrado.");

        // Atualiza os campos do hábito existente
        habitoExistente.Nome = habitoAtualizado.Nome;
        habitoExistente.Descricao = habitoAtualizado.Descricao;

        ctx.SaveChanges();
    
     return Results.Ok(habitoExistente);
    });

//SERVIÇO DE STREAKS
// POST: /api/registros
app.MapPost("/api/registros",
    ([FromServices] HabitTrackerContext db, 
     [FromBody] RegistroDiario novoRegistro, 
     [FromServices] StreakService streakService) =>
{
    var habitoId = novoRegistro.HabitoId;

    var habito = db.Habitos.FirstOrDefault(h => h.Id == habitoId);
    if (habito == null)
    {
        return Results.NotFound("Hábito não encontrado!");
    }

    var usuarioId = habito.UsuarioId;
    var hoje = DateTime.Today;

    var registroHoje = db.RegistrosDiarios
        .FirstOrDefault(r =>
            
 r.HabitoId == habitoId &&
            r.Data.Date == hoje);

    // Se já estava concluído, não dá erro — retorna streak normalmente
    if (registroHoje != null && registroHoje.Cumprido)
    {
        var (mensagemStreak, streakAtual) = streakService.VerificarConclusaoDiaria(usuarioId);

        return Results.Ok(new
        {
            mensagem = $"Hábito '{habito.Nome}' já estava concluído hoje.",
            infoStreak = mensagemStreak,
            streak = streakAtual
  
       });
    }

    // Caso contrário, marca como concluído
    if (registroHoje == null)
    {
        registroHoje = new RegistroDiario
        {
            HabitoId = habitoId,
            Data = DateTime.Now,
            Cumprido = true
        };
        db.RegistrosDiarios.Add(registroHoje);
    }
    else
    {
        registroHoje.Cumprido = true;
        db.RegistrosDiarios.Update(registroHoje);
    }

    db.SaveChanges();
 var (mensagemFinal, streakFinal) = streakService.VerificarConclusaoDiaria(usuarioId);

    return Results.Ok(new
    {
        mensagem = $"Hábito '{habito.Nome}' marcado como concluído!",
        infoStreak = mensagemFinal,
        streak = streakFinal
    });
});


// GET: /api/usuarios/{usuarioId}/streaks
app.MapGet("/api/usuarios/{usuarioId}/streaks",
    ([FromServices] HabitTrackerContext db, [FromRoute] int usuarioId) =>
    {
        var usuario = db.Usuarios.FirstOrDefault(u => u.Id == usuarioId);
        if (usuario == null)
        {
            return Results.NotFound("Usuário não encontrado!");
        
 }

        var hoje = DateTime.Today;
               var habitosComStatus = db.Habitos
            .Where(h => h.UsuarioId == usuarioId)
            .Select(h => new
            {
                h.Id,
                h.Nome,
                h.Descricao,
    
             h.UsuarioId,
                h.CriadoEm,
                ConcluidoHoje = db.RegistrosDiarios
                                  .Any(r => r.HabitoId == h.Id && r.Data.Date == hoje && r.Cumprido)
            })
            .ToList();

        // ALTERAÇÃO APLICADA AQUI: Retorna o Streak e a lista de Hábitos em uma estrutura única.
        return Results.Ok(new
        {
            streakTotal = usuario.Streak,
            habitosComStatus = habitosComStatus
        });
    });

//USUARIO SERVICE

//POST: /api/usuario/cadastrar
app.MapPost("/api/usuario/cadastrar",
    ([FromBody] Usuario usuario,
    [FromServices] HabitTrackerContext db) =>
{

    Usuario? resultado =
        db.Usuarios.FirstOrDefault(x => x.Email == usuario.Email);
    if (resultado is not null)
    {
    
     return Results.Conflict("Esse usuário já existe!");
    }

    usuario.Senha = BCrypt.Net.BCrypt.HashPassword(usuario.Senha);
    db.Usuarios.Add(usuario);
    db.SaveChanges();
    return Results.Created("", usuario);
});

//POST: /api/usuario/login
app.MapPost("/api/usuario/login",
    ([FromBody] LoginInputModel login, [FromServices] UsuarioService usuarioService) =>
    {
        var usuario = usuarioService.Autenticar(login);

        if (usuario == null)
        {
            return Results.Conflict("Email ou senha inválidos.");
        }
        return Results.Ok(new
        {
    
         Id = usuario.Id,
            Nome = usuario.Nome,
            Email = usuario.Email,
            Streak = usuario.Streak
        });
    });


app.UseCors("Acesso Total");
app.Run();