using System.Net;
using System.Net.Sockets;
using System.Text;
using epjb.Common;

namespace epjb.Cliente.Rede;

public static class DiagnosticoUdp
{
    // Retorna somente após receber o PONG correto. A tela chama via Task.Run para não bloquear os controles.
    public static void Testar(ConfiguracaoServidor config)
    {
        config.Validar();
        // O listener do servidor é IPv4. Resolução DNS também tem prazo, não apenas o recebimento.
        var enderecos = Dns.GetHostAddressesAsync(config.Host)
            .WaitAsync(TimeSpan.FromSeconds(3)).GetAwaiter().GetResult();
        var ip = enderecos.FirstOrDefault(x => x.AddressFamily == AddressFamily.InterNetwork)
            ?? throw new IOException("O diagnóstico UDP requer um endereço IPv4.");
        var destino = new IPEndPoint(ip, ProtocoloUdp.Porta);
        using var socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
        socket.SendTimeout = 3000;
        socket.Bind(new IPEndPoint(IPAddress.Any, 0)); // Porta efêmera para receber a resposta.
        string nonce = Guid.NewGuid().ToString("N");
        byte[] pedido = Encoding.UTF8.GetBytes(ProtocoloUdp.Ping + nonce);
        socket.SendTo(pedido, destino);
        byte[] buffer = new byte[65507];
        var prazo = System.Diagnostics.Stopwatch.StartNew();
        // Datagramas alheios não reiniciam o prazo: o diagnóstico inteiro tem limite de três segundos.
        while (prazo.ElapsedMilliseconds < 3000)
        {
            int restante = (int)(3000 - prazo.ElapsedMilliseconds);
            if (restante <= 0 || !socket.Poll(restante * 1000, SelectMode.SelectRead)) break;
            EndPoint remetente = new IPEndPoint(IPAddress.Any, 0);
            int lidos = socket.ReceiveFrom(buffer, ref remetente);
            string resposta = Encoding.UTF8.GetString(buffer, 0, lidos);
            // Um servidor ligado a 0.0.0.0 pode responder por outro IP da mesma máquina.
            // Correlacionamos o datagrama pela porta e nonce imprevisível, sem tratá-lo como autenticação.
            if (remetente is IPEndPoint origem && origem.Port == destino.Port
                && resposta == ProtocoloUdp.Pong + nonce) return;
        }
        // Ausência de PONG pode ser perda/bloqueio UDP; não prova que TCP ou o banco estejam indisponíveis.
        throw new IOException($"Sem resposta UDP de {destino}. Confira ServerSide e a porta UDP {ProtocoloUdp.Porta}. O teste TCP é independente.");
    }
}
