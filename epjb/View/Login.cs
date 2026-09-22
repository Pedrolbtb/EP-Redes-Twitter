using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;

namespace epjb.View
{
    public partial class Login : Form
    {

        public class ObjectState
        {
            public const int bufferSize = 256;
            public Socket wSocket = null;
            public byte[] buffer = new byte[bufferSize];
            public StringBuilder sb = new StringBuilder();
        }

        public class AsyncSocketClient
        {
            private const int Port = 4343;
            private static ManualResetEvent connectCompleted = new ManualResetEvent(false);
            private static ManualResetEvent sendCompleted = new ManualResetEvent(false);
            private static ManualResetEvent receiveCompleted = new ManualResetEvent(false);
            private static string respose = String.Empty;

            public static void StartClient(string json)
            {
                try
                {
                    IPHostEntry ipHost = Dns.GetHostEntry(Dns.GetHostName());
                    IPAddress ip = ipHost.AddressList[0];
                    IPEndPoint remoteEndPoint = new IPEndPoint(ip, Port);
                    Socket client = new Socket(ip.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                    client.BeginConnect(remoteEndPoint, new AsyncCallback(ConnectCallback), client);
                    connectCompleted.WaitOne();
                    Send(client, "Essa eh uma mensagem de socket");
                    sendCompleted.WaitOne();
                    Receive(client);
                    receiveCompleted.WaitOne();
                    Console.WriteLine("Resposta do servidor: {0}", respose);
                    client.Shutdown(SocketShutdown.Both);
                    client.Close();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                }
            }

            private static void Receive(Socket client)
            {
                try
                {
                    ObjectState state = new ObjectState();
                    state.wSocket = client;
                    client.BeginReceive(state.buffer, 0, ObjectState.bufferSize, 0, new AsyncCallback(ReceiveCallback), state);

                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }

            private static void ReceiveCallback(IAsyncResult ar)
            {
                try
                {
                    ObjectState state = (ObjectState)ar.AsyncState;
                    var client = state.wSocket;
                    int bytesRead = client.EndReceive(ar);
                    if (bytesRead > 0)
                    {
                        state.sb.Append(Encoding.ASCII.GetString(state.buffer, 0, bytesRead));
                        client.BeginReceive(state.buffer, 0, ObjectState.bufferSize, 0, new AsyncCallback(ReceiveCallback), state);

                    }
                    else
                    {
                        if (state.sb.Length > 1)
                        {
                            respose = state.sb.ToString();
                        }
                        receiveCompleted.Set();
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }

            private static void Send(Socket client, string message)
            {
                byte[] byteData = Encoding.ASCII.GetBytes(message);
                client.BeginSend(byteData, 0, byteData.Length, 0, new AsyncCallback(SendCallback), client);
            }

            private static void SendCallback(IAsyncResult ar)
            {
                try
                {
                    Socket client = (Socket)ar.AsyncState;
                    int bytesSent = client.EndSend(ar);
                    sendCompleted.Set();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }

            private static void ConnectCallback(IAsyncResult ar)
            {
                try
                {
                    Socket client = (Socket)ar.AsyncState;
                    client.EndConnect(ar);
                    connectCompleted.Set();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
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
            AsyncSocketClient.StartClient(json);
        }
    }
}