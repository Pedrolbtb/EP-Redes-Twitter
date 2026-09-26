using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System.Windows.Forms;
using epjb.Common;

namespace epjb.View
{
    public partial class Login : Form
    {

        public class AsyncSocketClient
        {
            private const int Port = Config.Porta;

            public static void StartClient(string json, Form owner)
            {
                // Execute em background para não bloquear a UI
                System.Threading.Tasks.Task.Run(() =>
                {
                    try
                    {
                        Console.WriteLine("[Client] Antes de obter IP/endpoint");
                        IPEndPoint remoteEndPoint = new IPEndPoint(IPAddress.Loopback, Port);
                        Console.WriteLine($"[Client] Endpoint: {remoteEndPoint}");
                        using (Socket client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp))
                        {
                            Console.WriteLine("[Client] Antes de conectar");
                            client.Connect(remoteEndPoint);
                            Console.WriteLine("[Client] Conectado");

                            // Envia Mensagem usando o Protocolo
                            Console.WriteLine("[Client] Antes de enviar");
                            Protocolo.Enviar(client, new Mensagem(Comando.LOGIN, json));
                            Console.WriteLine("[Client] Depois de enviar");

                            // Recebe resposta
                            Console.WriteLine("[Client] Antes de receber");
                            Mensagem resposta = Protocolo.Receber(client);
                            Console.WriteLine("[Client] Depois de receber");

                            if (resposta == null)
                            {
                                owner?.Invoke(() => MessageBox.Show(owner, "Sem resposta do servidor."));
                            }
                            else if (resposta.Sucesso)
                            {
                                owner?.Invoke(() => MessageBox.Show(owner, "Operação bem-sucedida."));
                            }
                            else
                            {
                                owner?.Invoke(() => MessageBox.Show(owner, $"Erro: {resposta.Erro}"));
                            }
                            try { client.Shutdown(SocketShutdown.Both); } catch { }
                            client.Close();
                        }
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("ERRO: " + e.ToString());
                        owner?.Invoke(() => MessageBox.Show(owner, e.ToString()));
                    }
                });
            }
        }
        public Login()
        {
            InitializeComponent();
        }

        private void btnFechar_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            var payload = new
            {
                username = txtUsuario.Text,
                password = txtSenha.Text
            };

            string json = JsonSerializer.Serialize(payload);
            AsyncSocketClient.StartClient(json, this);
        }
    }
}