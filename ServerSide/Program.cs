using epjb.Data;
using epjb.Sockets;
using Microsoft.EntityFrameworkCore;

try
{
    Console.WriteLine($"Banco central: {AppDbContext.CaminhoBanco}");
    using (var db = new AppDbContext())
        db.Database.Migrate();
    AsyncSocketListener.StartListener();
}
catch (Exception ex)
{
    Console.Error.WriteLine($"Não foi possível iniciar o servidor: {ex.Message}");
    Environment.ExitCode = 1;
}
