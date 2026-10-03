using System.Net;
using System.Net.Sockets;
using System.Text;
using epjb.Common;

// Exercita o framing com sockets reais; não usa mocks nem pressupõe que Receive devolve uma mensagem inteira.
internal static class ProtocoloTests
{
    public static async Task Executar()
    {
        // Envia um quadro byte a byte, inclusive o cabeçalho: o receptor deve remontá-lo.
        await ComPar((a, b) =>
        {
            byte[] json = Encoding.UTF8.GetBytes("{\"Tipo\":0,\"PayloadJson\":\"{}\",\"Sucesso\":true}");
            byte[] quadro = BitConverter.GetBytes(IPAddress.HostToNetworkOrder(json.Length)).Concat(json).ToArray();
            var escrita = Task.Run(() => { foreach (byte item in quadro) a.Send(new[] { item }); });
            if (Protocolo.Receber(b)?.Sucesso != true) throw new Exception("Falha com quadro fragmentado.");
            escrita.GetAwaiter().GetResult();
        });
        // Dois quadros seguidos não podem ser fundidos. Um payload grande exercita os loops de envio/leitura.
        await ComPar((a, b) =>
        {
            string longo = new('x', 256 * 1024);
            var escrita = Task.Run(() =>
            {
                Protocolo.Enviar(a, new Mensagem(Comando.LOGIN, longo));
                Protocolo.Enviar(a, new Mensagem(Comando.CADASTRO, "segundo"));
                a.Shutdown(SocketShutdown.Send);
            });
            if (Protocolo.Receber(b)?.PayloadJson != longo || Protocolo.Receber(b)?.PayloadJson != "segundo")
                throw new Exception("Quadros consecutivos corrompidos.");
            if (Protocolo.Receber(b) != null) throw new Exception("EOF normal deveria retornar null.");
            escrita.GetAwaiter().GetResult();
        });
        // Um peer não pode forçar alocação acima do limite nem enviar tamanho negativo/zero.
        foreach (int tamanho in new[] { -1, 0, Protocolo.TamanhoMaximo + 1 })
            await ComPar((a, b) =>
            {
                a.Send(BitConverter.GetBytes(IPAddress.HostToNetworkOrder(tamanho)));
                EsperarErro<InvalidDataException>(() => Protocolo.Receber(b));
            });
        // Encerramento no meio do cabeçalho ou do payload precisa ser reportado como quadro truncado.
        await ComPar((a, b) =>
        {
            a.Send(new byte[] { 0, 0 });
            a.Shutdown(SocketShutdown.Send);
            EsperarErro<EndOfStreamException>(() => Protocolo.Receber(b));
        });
        await ComPar((a, b) =>
        {
            a.Send(new byte[] { 0, 0, 0, 8, (byte)'{' });
            a.Shutdown(SocketShutdown.Send);
            EsperarErro<EndOfStreamException>(() => Protocolo.Receber(b));
        });
        Console.WriteLine("PASSOU: TCP fragmentado, quadros consecutivos, payload grande, limites e EOF.");
    }

    private static void EsperarErro<T>(Action acao) where T : Exception
    {
        try { acao(); }
        catch (T) { return; }
        throw new Exception($"Era esperado {typeof(T).Name}.");
    }

    private static async Task ComPar(Action<Socket, Socket> teste)
    {
        // Porta efêmera evita colisão com o servidor de integração.
        using var listener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        listener.Bind(new IPEndPoint(IPAddress.Loopback, 0));
        listener.Listen(1);
        using var cliente = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        await cliente.ConnectAsync(listener.LocalEndPoint!);
        using var servidor = listener.Accept();
        cliente.SendTimeout = servidor.ReceiveTimeout = 5000;
        teste(cliente, servidor);
    }
}
