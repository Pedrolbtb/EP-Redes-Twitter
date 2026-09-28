using System.Text.Json;
using epjb.Common;

namespace epjb.Cliente.Rede;

public sealed class ConfiguracaoServidor
{
    public string Host { get; set; } = "localhost";
    public int Porta { get; set; } = Config.Porta;

    public static ConfiguracaoServidor Carregar()
    {
        string arquivo = Path.Combine(AppContext.BaseDirectory, "servidor.json");
        var config = File.Exists(arquivo)
            ? JsonSerializer.Deserialize<ConfiguracaoServidor>(File.ReadAllText(arquivo),
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true })
                ?? throw new InvalidOperationException("servidor.json está vazio.")
            : new ConfiguracaoServidor();
        config.Host = Environment.GetEnvironmentVariable("EPJB_SERVER_HOST") ?? config.Host;
        if (string.IsNullOrWhiteSpace(config.Host) || config.Porta < 1 || config.Porta > 65535)
            throw new InvalidOperationException("Configure Host e Porta válidos em servidor.json.");
        config.Host = config.Host.Trim();
        return config;
    }
}
