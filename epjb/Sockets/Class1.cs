using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading;
using System.Threading.Tasks;
using epjb.Common;

namespace epjb.Sockets
{
    public class AsyncSocketListener
    {
        public static ManualResetEvent allDone = new ManualResetEvent(false);
        public static void StartListener()
        {
            IPEndPoint localEndPoint = new IPEndPoint(IPAddress.Any, Config.Porta);
            Socket listener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            try
            {
                listener.Bind(localEndPoint);
                listener.Listen(100);

                while (true)
                {
                    allDone.Reset();
                    Console.WriteLine("Aguardando conexão...");
                    listener.BeginAccept(new AsyncCallback(AcceptCallback), listener);
                    allDone.WaitOne();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            Console.WriteLine("Pressione ENTER para continuar...");
            Console.ReadLine();
        }

        private static void AcceptCallback(IAsyncResult ar)
        {
            allDone.Set();
            Socket listener = (Socket)ar.AsyncState;
            Socket handler = listener.EndAccept(ar);

            // Rode um loop de atendimento por cliente em uma Task separada,
            // usando Protocolo.Receber / Protocolo.Enviar.
            Task.Run(() => HandleClient(handler));
        }

        private static void HandleClient(Socket handler)
        {
            try
            {
                while (true)
                {
                    Mensagem msg = Protocolo.Receber(handler);
                    if (msg == null) break; // cliente desconectou
                    Console.WriteLine($"Recebido comando: {msg.Tipo}");
                    // exemplo: eco de sucesso com mesmo payload
                    Protocolo.Enviar(handler, Mensagem.RespostaSucesso(msg.Tipo, msg.PayloadJson));
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro no cliente: {ex}");
            }
            finally
            {
                try { handler.Shutdown(SocketShutdown.Both); } catch { }
                try { handler.Close(); } catch { }
            }
        }
    }
}
