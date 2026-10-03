using System.Text.Json;
using epjb.Cliente.Rede;

internal static class ConfiguracaoTests
{
    public static void Executar()
    {
        // O arquivo pertence ao executável de TESTE. Preservar configuração anterior permite rodar novamente.
        string caminho = ConfiguracaoServidor.CaminhoArquivo;
        byte[]? original = File.Exists(caminho) ? File.ReadAllBytes(caminho) : null;
        string? ambiente = Environment.GetEnvironmentVariable("EPJB_SERVER_HOST");
        try
        {
            Environment.SetEnvironmentVariable("EPJB_SERVER_HOST", null);
            new ConfiguracaoServidor { Host = "192.168.1.50", Porta = 11000 }.Salvar();
            if (ConfiguracaoServidor.Carregar().Host != "192.168.1.50") throw new Exception("Destino salvo não foi usado.");
            Environment.SetEnvironmentVariable("EPJB_SERVER_HOST", "127.0.0.2");
            if (ConfiguracaoServidor.Carregar().Host != "127.0.0.2") throw new Exception("Variável não prevaleceu sobre JSON.");
            // Validar antes da escrita deve manter a configuração válida em caso de erro de entrada.
            try
            {
                new ConfiguracaoServidor { Host = "http://ip-invalido", Porta = 0 }.Salvar();
                throw new Exception("Configuração inválida foi aceita.");
            }
            catch (InvalidOperationException) { }
            Environment.SetEnvironmentVariable("EPJB_SERVER_HOST", null);
            if (ConfiguracaoServidor.Carregar().Host != "192.168.1.50") throw new Exception("Arquivo válido foi sobrescrito.");
            File.WriteAllText(caminho, "{json quebrado");
            try { ConfiguracaoServidor.Carregar(); throw new Exception("JSON inválido foi aceito."); }
            catch (JsonException) { }
            // É o mesmo caminho utilizado pelo botão Salvar para recuperar um arquivo inválido.
            new ConfiguracaoServidor { Host = "localhost" }.Salvar();
            if (ConfiguracaoServidor.Carregar().Host != "localhost") throw new Exception("Recuperação do JSON falhou.");
            Console.WriteLine("PASSOU: persistência da configuração, prioridade da variável e recuperação de JSON.");
        }
        finally
        {
            if (original == null) File.Delete(caminho); else File.WriteAllBytes(caminho, original);
            Environment.SetEnvironmentVariable("EPJB_SERVER_HOST", ambiente);
        }
    }
}
