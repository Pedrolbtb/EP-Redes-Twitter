using System.Diagnostics;
using System.Net.Sockets;
using System.Text.Json;
using epjb.Cliente.Rede;

if (args.Length != 1) throw new ArgumentException("Informe o caminho absoluto de ServerSide.dll já compilado.");
// Primeiro verifica o transporte isoladamente, sem depender do banco ou da porta fixa do servidor.
await ProtocoloTests.Executar();
ConfiguracaoTests.Executar();
string servidorDll = Path.GetFullPath(args[0]);
string temporario = Path.Combine(Path.GetTempPath(), "epjb-integracao-" + Guid.NewGuid());
Directory.CreateDirectory(temporario);
string banco = Path.Combine(temporario, "central.db");
Environment.SetEnvironmentVariable("EPJB_SERVER_HOST", "127.0.0.2");
using var api = new ServicoApi();
Process? servidor = null;
try
{
    // Não usar ou encerrar um servidor de outra pessoa na porta de teste.
    using (var probe = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp))
        probe.Bind(new System.Net.IPEndPoint(System.Net.IPAddress.Any, 11000));
    // Se UDP estiver ocupado por outro processo, falhar antes de iniciar o teste, sem interferir nele.
    using (var probeUdp = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp))
        probeUdp.Bind(new System.Net.IPEndPoint(System.Net.IPAddress.Any, epjb.Common.ProtocoloUdp.Porta));

    // A recusa precisa orientar o usuário e preservar o destino, sem criar servidor/banco no cliente.
    var indisponivel = await api.LoginAsync("ninguem", "senha");
    if (indisponivel.Sucesso || !indisponivel.Erro.Contains("127.0.0.2:11000") || !indisponivel.Erro.Contains("ServerSide"))
        throw new Exception("Diagnóstico de conexão recusada incompleto.");
    await Iniciar();
    // Demonstra PING/PONG com Socket.SendTo/ReceiveFrom, além dos comandos de negócio via TCP.
    DiagnosticoUdp.Testar(new ConfiguracaoServidor { Host = "127.0.0.2" });
    // As duas requisições usam sockets separados mas gravam no mesmo arquivo central.
    var cadastros = await Task.WhenAll(api.CadastroAsync("pessoa_a", "senha123"), api.CadastroAsync("pessoa_b", "senha456"));
    foreach (var resposta in cadastros) Validar(resposta);
    int a = Validar(await api.LoginAsync("pessoa_a", "senha123")).GetProperty("id").GetInt32();
    int b = Validar(await api.LoginAsync("pessoa_b", "senha456")).GetProperty("id").GetInt32();
    if (a == b) throw new Exception("Usuários distintos receberam o mesmo ID.");
    var posts = await Task.WhenAll(api.PostarMensagemAsync(a, "Mensagem do PC A"), api.PostarMensagemAsync(b, "Mensagem do PC B"));
    foreach (var resposta in posts) Validar(resposta);
    Validar(await api.SeguirUsuarioAsync(a, b));
    // O mesmo conjunto de posts precisa estar visível em ambas as conexões.
    var feeds = await Task.WhenAll(api.ListarMensagensAsync(), api.ListarMensagensAsync());
    foreach (var feed in feeds)
        if (Validar(feed).GetArrayLength() != 2) throw new Exception("Feed não compartilhado.");
    if (Validar(await api.ListarMeusSeguindoAsync(a)).GetArrayLength() != 1) throw new Exception("Relação não persistida.");
    Parar();
    // O cliente UDP deve encerrar por prazo quando o servidor não responder.
    try
    {
        DiagnosticoUdp.Testar(new ConfiguracaoServidor { Host = "127.0.0.2" });
        throw new Exception("UDP respondeu mesmo sem servidor.");
    }
    catch (IOException) { }
    catch (SocketException) { } // Alguns sistemas entregam imediatamente ICMP de porta inalcançável.
    // Reiniciar em outro diretório deve continuar usando o mesmo banco explícito.
    await Iniciar(Path.GetTempPath());
    Validar(await api.LoginAsync("pessoa_a", "senha123"));
    if (Validar(await api.ListarMensagensAsync()).GetArrayLength() != 2) throw new Exception("Dados perdidos ao reiniciar.");
    if (Directory.GetFiles(temporario, "*.db", SearchOption.AllDirectories).Length != 1)
        throw new Exception("Mais de um banco foi criado.");
    Console.WriteLine("PASSOU: conexão recusada orientada, UDP PING/PONG e indisponibilidade, clientes simultâneos e persistência.");
}
finally
{
    Parar();
    // Limpar também os pools locais não é necessário: apenas o servidor abre SQLite.
    Directory.Delete(temporario, true);
}

// Falhas de negócio interrompem o teste com a mensagem real devolvida pelo servidor.
JsonElement Validar(epjb.Common.Mensagem resposta)
{
    if (resposta?.Sucesso != true) throw new Exception(resposta?.Erro ?? "Sem resposta");
    return JsonSerializer.Deserialize<JsonElement>(resposta.PayloadJson ?? "null");
}

// Sobe um processo isolado e aguarda a confirmação de Listen antes de enviar comandos.
async Task Iniciar(string? diretorio = null)
{
    var info = new ProcessStartInfo(Environment.ProcessPath!)
    {
        WorkingDirectory = diretorio ?? temporario,
        RedirectStandardOutput = true,
        RedirectStandardError = true,
        UseShellExecute = false
    };
    // Ao executar via 'dotnet Integracao.dll', ProcessPath é o host dotnet.
    info.ArgumentList.Add(servidorDll);
    info.Environment["EPJB_DB_PATH"] = banco;
    servidor = Process.Start(info)!;
    var pronto = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
    servidor.OutputDataReceived += (_, e) =>
    {
        if (e.Data?.Contains("Servidor TCP aguardando") == true) pronto.TrySetResult();
    };
    servidor.ErrorDataReceived += (_, e) => { if (e.Data != null) Console.Error.WriteLine(e.Data); };
    servidor.BeginOutputReadLine();
    servidor.BeginErrorReadLine();
    // Se o processo morrer antes de Listen, mostrar a falha sem esperar o timeout inteiro.
    var encerrado = servidor.WaitForExitAsync();
    var concluido = await Task.WhenAny(pronto.Task, encerrado).WaitAsync(TimeSpan.FromSeconds(30));
    if (concluido == encerrado) throw new Exception($"Servidor encerrou antes de Listen (código {servidor.ExitCode}).");
    await pronto.Task;
}

// Encerra somente o processo criado por este teste, inclusive em caso de asserção falha.
void Parar()
{
    if (servidor == null) return;
    if (!servidor.HasExited) servidor.Kill(entireProcessTree: true);
    servidor.WaitForExit();
    servidor.Dispose();
    servidor = null;
}
