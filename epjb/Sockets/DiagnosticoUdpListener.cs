using System.Net;
using System.Net.Sockets;
using System.Text;
using epjb.Common;

namespace epjb.Sockets;

// Demonstra Socket UDP manual. É independente do TCP e não acessa o banco de dados.
public sealed class DiagnosticoUdpListener : IDisposable
{
    private readonly Socket socket;

    public DiagnosticoUdpListener()
    {
        // Dgram + Udp preserva as fronteiras de cada datagrama; não existe Listen/Accept no UDP.
        socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
        try { socket.Bind(new IPEndPoint(IPAddress.Any, ProtocoloUdp.Porta)); }
        catch { socket.Dispose(); throw; }
    }

    public void Executar()
    {
        // O buffer comporta o datagrama IPv4 máximo. Mensagens fora do formato são descartadas.
        byte[] buffer = new byte[65507];
        Console.WriteLine($"Diagnóstico UDP aguardando em 0.0.0.0:{ProtocoloUdp.Porta}");
        while (true)
        {
            try
            {
                // ReceiveFrom devolve quantos bytes chegaram e o endereço de QUEM enviou.
                EndPoint remetente = new IPEndPoint(IPAddress.Any, 0);
                int lidos = socket.ReceiveFrom(buffer, ref remetente);
                string pedido = Encoding.UTF8.GetString(buffer, 0, lidos);
                if (!pedido.StartsWith(ProtocoloUdp.Ping, StringComparison.Ordinal)) continue;
                string nonce = pedido[ProtocoloUdp.Ping.Length..];
                if (nonce.Length != ProtocoloUdp.TamanhoNonce || !Guid.TryParseExact(nonce, "N", out _)) continue;
                byte[] resposta = Encoding.UTF8.GetBytes(ProtocoloUdp.Pong + nonce);
                // SendTo responde em um único datagrama ao IP/porta do remetente, sem conexão TCP.
                socket.SendTo(resposta, remetente);
            }
            catch (ObjectDisposedException) { return; }
            catch (SocketException ex)
            {
                // Fechar o socket cancela ReceiveFrom; erros de rede pontuais não encerram o servidor TCP.
                if (socket.SafeHandle.IsClosed) return;
                Console.Error.WriteLine($"[UDP] {ex.SocketErrorCode}: {ex.Message}");
            }
        }
    }

    // Libera a porta e desbloqueia ReceiveFrom quando o processo encerra o serviço.
    public void Dispose() => socket.Dispose();
}
