using System.Text.Json;
using epjb.Common;

namespace epjb.Cliente.Rede;

// Guarda somente o destino da conexão. Nenhum caminho de banco pertence ao cliente.
public sealed class ConfiguracaoServidor
{
    public string Host { get; set; } = "localhost";
    public int Porta { get; set; } = Config.Porta;

    // Usar a pasta do executável evita procurar um JSON diferente a cada diretório de execução.
    public static string CaminhoArquivo => Path.Combine(AppContext.BaseDirectory, "servidor.json");

    public static ConfiguracaoServidor Carregar()
    {
        // A ausência do arquivo mantém o teste local; JSON inválido é um erro de configuração.
        var config = File.Exists(CaminhoArquivo)
            ? JsonSerializer.Deserialize<ConfiguracaoServidor>(File.ReadAllText(CaminhoArquivo),
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true })
                ?? throw new InvalidOperationException("servidor.json está vazio. Abra Servidor... na tela de login para corrigir.")
            : new ConfiguracaoServidor();

        // A variável permite configurar o destino sem alterar os arquivos publicados.
        config.Host = Environment.GetEnvironmentVariable("EPJB_SERVER_HOST") ?? config.Host;
        config.Validar();
        return config;
    }

    public void Validar()
    {
        // Recebemos um nome/IP e uma porta separados, nunca uma URL HTTP nem 'IP:porta'.
        if (string.IsNullOrWhiteSpace(Host) || Uri.CheckHostName(Host.Trim()) == UriHostNameType.Unknown
            || Porta < 1 || Porta > 65535)
            throw new InvalidOperationException("Informe um IP/nome de servidor sem http:// e uma porta entre 1 e 65535.");
        Host = Host.Trim();
    }

    public void Salvar()
    {
        Validar();
        // Só gravamos após validar; se a pasta for protegida, a tela informa a falha sem perder o formulário.
        File.WriteAllText(CaminhoArquivo, JsonSerializer.Serialize(this, new JsonSerializerOptions { WriteIndented = true }));
    }
}
