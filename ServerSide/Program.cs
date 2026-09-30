using epjb.Data;
using epjb.Sockets;
using Microsoft.EntityFrameworkCore;

try
{
    // Este processo é o único dono do SQLite. O cliente não deve executar migrations.
    Console.WriteLine($"Banco central: {AppDbContext.CaminhoBanco}");
    // Concluir as migrations ANTES de anunciar prontidão evita aceitar logins sem tabelas.
    using (var db = new AppDbContext())
        db.Database.Migrate();
    // UDP é diagnóstico opcional: uma falha ao abrir sua porta não impede login/feed por TCP.
    using var udp = IniciarUdp();
    Console.WriteLine("Mantenha esta janela aberta. Nos clientes, use Servidor... para informar o IPv4 deste PC.");
    // Daqui em diante o loop manual de Bind/Listen/Accept mantém o servidor em execução.
    AsyncSocketListener.StartListener();
}
catch (Exception ex)
{
    // Ex.: porta ocupada ou banco inacessível. Código 1 permite detectar falha em scripts.
    Console.Error.WriteLine($"Não foi possível iniciar o servidor: {ex.Message}");
    Environment.ExitCode = 1;
}

// Bind UDP é feito antes da tarefa para reportar imediatamente porta ocupada/permissão negada.
static DiagnosticoUdpListener? IniciarUdp()
{
    try
    {
        var listener = new DiagnosticoUdpListener();
        _ = Task.Run(listener.Executar);
        return listener;
    }
    catch (Exception ex)
    {
        Console.Error.WriteLine($"Diagnóstico UDP indisponível; TCP continuará: {ex.Message}");
        return null;
    }
}
