using System.Net.Sockets;
using System.Text;
using System.Text.Json;

namespace epjb.Common;

// Protocolo TCP programado pelo grupo: [4 bytes de tamanho big-endian][JSON UTF-8].
// TCP entrega um fluxo de bytes, não mensagens: Send/Receive podem transferir apenas parte do quadro.
public static class Protocolo
{
    // Rejeitar tamanhos inválidos impede alocar memória ilimitada a partir de um cabeçalho recebido.
    public const int TamanhoMaximo = 4 * 1024 * 1024;

    public static void Enviar(Socket socket, Mensagem mensagem)
    {
        // JSON só representa os dados; quem transporta esses bytes é Socket.Send abaixo.
        byte[] payload = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(mensagem));
        if (payload.Length > TamanhoMaximo) throw new InvalidDataException("Mensagem excede 4 MiB.");
        byte[] tamanho = BitConverter.GetBytes(payload.Length);
        // A ordem de rede é big-endian, independentemente da arquitetura de cada PC.
        if (BitConverter.IsLittleEndian) Array.Reverse(tamanho);
        EnviarExato(socket, tamanho);
        EnviarExato(socket, payload);
    }

    public static Mensagem? Receber(Socket socket)
    {
        // EOF antes do primeiro byte significa encerramento normal entre mensagens.
        byte[]? cabecalho = ReceberExato(socket, 4, permitirFim: true);
        if (cabecalho == null) return null;
        if (BitConverter.IsLittleEndian) Array.Reverse(cabecalho);
        int tamanho = BitConverter.ToInt32(cabecalho, 0);
        if (tamanho <= 0 || tamanho > TamanhoMaximo)
            throw new InvalidDataException("Tamanho do quadro TCP inválido (limite: 4 MiB).");
        // Ler exatamente esse tamanho também preserva mensagens seguintes que já estejam no socket.
        byte[] payload = ReceberExato(socket, tamanho)!;
        return JsonSerializer.Deserialize<Mensagem>(Encoding.UTF8.GetString(payload))
            ?? throw new InvalidDataException("O quadro TCP não contém uma mensagem.");
    }

    private static void EnviarExato(Socket socket, byte[] buffer)
    {
        int enviados = 0;
        while (enviados < buffer.Length)
        {
            // A chamada retorna quantos bytes foram aceitos; reenviamos apenas a parte restante.
            int quantidade = socket.Send(buffer, enviados, buffer.Length - enviados, SocketFlags.None);
            if (quantidade == 0) throw new IOException("Conexão encerrada durante o envio.");
            enviados += quantidade;
        }
    }

    private static byte[]? ReceberExato(Socket socket, int quantidade, bool permitirFim = false)
    {
        byte[] buffer = new byte[quantidade];
        int totalLido = 0;
        while (totalLido < quantidade)
        {
            // Receive bloqueia até chegar algum byte, ocorrer erro/timeout ou o outro lado fechar.
            int lido = socket.Receive(buffer, totalLido, quantidade - totalLido, SocketFlags.None);
            if (lido == 0)
            {
                if (permitirFim && totalLido == 0) return null;
                // Fechar no meio de um quadro não pode ser confundido com uma mensagem válida.
                throw new EndOfStreamException("Conexão encerrada no meio de um quadro TCP.");
            }
            totalLido += lido;
        }
        return buffer;
    }
}
