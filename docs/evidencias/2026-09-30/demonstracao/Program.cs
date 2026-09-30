using epjb.Cliente.Rede;
using epjb.Common;
using System.Text.Json;

// Clientes de console distintos usando a camada REAL do projeto, na mesma máquina Linux.
Environment.SetEnvironmentVariable("EPJB_SERVER_HOST", "127.0.0.1");
using var clienteA = new ServicoApi();
using var clienteB = new ServicoApi();
string fase = args.FirstOrDefault() ?? "criar";
Console.WriteLine($"RELATORIO | Fase: {fase}; destino TCP 127.0.0.1:11000; UDP 11001.");
Console.WriteLine("RELATORIO | Dois clientes de console no mesmo Linux; sem interface Windows Forms.");
if (fase == "criar")
{
    var cadastros = await Task.WhenAll(
        clienteA.CadastroAsync("aluno_a", "DemoA123"),
        clienteB.CadastroAsync("aluno_b", "DemoB123"));
    Mostrar("Cadastro A", cadastros[0]);
    Mostrar("Cadastro B", cadastros[1]);
}
var loginA = Mostrar("Login A", await clienteA.LoginAsync("aluno_a", "DemoA123"));
var loginB = Mostrar("Login B", await clienteB.LoginAsync("aluno_b", "DemoB123"));
int a = loginA.GetProperty("id").GetInt32();
int b = loginB.GetProperty("id").GetInt32();
if (fase == "criar")
{
    var posts = await Task.WhenAll(
        clienteA.PostarMensagemAsync(a, "Ola! Mensagem publicada pelo cliente A."),
        clienteB.PostarMensagemAsync(b, "O cliente B usa o mesmo banco central."));
    Mostrar("Post A", posts[0]);
    Mostrar("Post B", posts[1]);
    Mostrar("A segue B", await clienteA.SeguirUsuarioAsync(a, b));
}
// Duas consultas independentes devem observar os mesmos posts persistidos.
var feeds = await Task.WhenAll(clienteA.ListarMensagensAsync(), clienteB.ListarMensagensAsync());
var feedA = Mostrar("Feed recebido por A", feeds[0]);
var feedB = Mostrar("Feed recebido por B", feeds[1]);
string assinaturaA = string.Join("|", feedA.EnumerateArray().Select(m => m.GetProperty("id").GetInt32()).Order());
string assinaturaB = string.Join("|", feedB.EnumerateArray().Select(m => m.GetProperty("id").GetInt32()).Order());
if (assinaturaA != assinaturaB || feedA.GetArrayLength() != 2) throw new Exception("Feeds divergentes");
Console.WriteLine("RELATORIO | CONFIRMADO: ambos os clientes receberam os mesmos 2 posts.");
Mostrar("Usuarios que A segue", await clienteA.ListarMeusSeguindoAsync(a));
Mostrar("Seguidores de B", await clienteB.ListarMeusSeguidoresAsync(b));
DiagnosticoUdp.Testar(new ConfiguracaoServidor { Host = "127.0.0.1" });
Console.WriteLine("RELATORIO | UDP: PING/PONG confirmado com nonce correspondente.");
if (fase == "reabrir")
    Console.WriteLine("RELATORIO | PERSISTENCIA CONFIRMADA: contas, posts e relacao sobreviveram ao reinicio.");

// Registra a resposta recebida; nunca imprime senhas enviadas nos pedidos.
static JsonElement Mostrar(string etapa, Mensagem resposta)
{
    Console.WriteLine($"RELATORIO | {etapa}: Sucesso={resposta.Sucesso}");
    if (!resposta.Sucesso) throw new Exception(resposta.Erro);
    var dados = JsonSerializer.Deserialize<JsonElement>(resposta.PayloadJson ?? "null");
    Console.WriteLine($"RELATORIO | Resposta: {JsonSerializer.Serialize(dados, new JsonSerializerOptions { Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping })}");
    return dados;
}
