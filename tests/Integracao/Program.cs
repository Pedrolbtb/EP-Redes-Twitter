using System.Diagnostics;
using System.Net.Sockets;
using System.Text.Json;
using epjb.Cliente.Rede;

if (args.Length != 1) throw new ArgumentException("Informe o caminho absoluto de ServerSide.dll já compilado.");
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
    await Iniciar();
    var cadastros = await Task.WhenAll(api.CadastroAsync("pessoa_a", "senha123"), api.CadastroAsync("pessoa_b", "senha456"));
    foreach (var resposta in cadastros) Validar(resposta);
    int a = Validar(await api.LoginAsync("pessoa_a", "senha123")).GetProperty("id").GetInt32();
    int b = Validar(await api.LoginAsync("pessoa_b", "senha456")).GetProperty("id").GetInt32();
    if (a == b) throw new Exception("Usuários distintos receberam o mesmo ID.");
    var posts = await Task.WhenAll(api.PostarMensagemAsync(a, "Mensagem do PC A"), api.PostarMensagemAsync(b, "Mensagem do PC B"));
    foreach (var resposta in posts) Validar(resposta);
    Validar(await api.SeguirUsuarioAsync(a, b));
    var feeds = await Task.WhenAll(api.ListarMensagensAsync(), api.ListarMensagensAsync());
    foreach (var feed in feeds)
        if (Validar(feed).GetArrayLength() != 2) throw new Exception("Feed não compartilhado.");
    if (Validar(await api.ListarMeusSeguindoAsync(a)).GetArrayLength() != 1) throw new Exception("Relação não persistida.");
    Parar();
    // Reiniciar em outro diretório deve continuar usando o mesmo banco explícito.
    await Iniciar(Path.GetTempPath());
    Validar(await api.LoginAsync("pessoa_a", "senha123"));
    if (Validar(await api.ListarMensagensAsync()).GetArrayLength() != 2) throw new Exception("Dados perdidos ao reiniciar.");
    if (Directory.GetFiles(temporario, "*.db", SearchOption.AllDirectories).Length != 1)
        throw new Exception("Mais de um banco foi criado.");
    Console.WriteLine("PASSOU: cadastros e posts simultâneos, login, feed compartilhado, seguir e persistência após reinício.");
}
finally
{
    Parar();
    // Limpar também os pools locais não é necessário: apenas o servidor abre SQLite.
    Directory.Delete(temporario, true);
}

JsonElement Validar(epjb.Common.Mensagem resposta)
{
    if (resposta?.Sucesso != true) throw new Exception(resposta?.Erro ?? "Sem resposta");
    return JsonSerializer.Deserialize<JsonElement>(resposta.PayloadJson ?? "null");
}

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
    await pronto.Task.WaitAsync(TimeSpan.FromSeconds(30));
}

void Parar()
{
    if (servidor == null) return;
    if (!servidor.HasExited) servidor.Kill(entireProcessTree: true);
    servidor.WaitForExit();
    servidor.Dispose();
    servidor = null;
}
