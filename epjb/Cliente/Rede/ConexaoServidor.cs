using System;
using System.Net;
using System.Net.Sockets;
using epjb.Common;

namespace epjb.Cliente.Rede
{
    /// <summary>
    /// Responsável pela conexão TCP com o servidor.
    /// Código do grupo: cria o Socket diretamente; Protocolo também usa Send/Receive manuais.
    /// </summary>
    public class ConexaoServidor : IDisposable
    {
        private Socket socket = null!;
        private readonly ConfiguracaoServidor? configuracao;

        // O teste da tela pode usar um destino ainda não salvo. As demais chamadas leem o JSON.
        public ConexaoServidor(ConfiguracaoServidor? configuracao = null)
        {
            this.configuracao = configuracao;
        }
        private bool conectado = false;

        public bool EstaConectado => conectado && socket?.Connected == true;

        /// <summary>
        /// Conecta ao endereço configurado em servidor.json ou EPJB_SERVER_HOST.
        /// </summary>
        public void Conectar()
        {
            var config = configuracao ?? ConfiguracaoServidor.Carregar();
            config.Validar();
            try
            {
                Console.WriteLine($"[ConexaoServidor] Conectando a {config.Host}:{config.Porta}...");
                // Stream + Tcp cria um socket TCP do sistema operacional, sem TcpClient/HTTP/RPC.
                // Esse construtor aceita IPv4 e, quando disponível, IPv6.
                socket = new Socket(SocketType.Stream, ProtocolType.Tcp)
                {
                    ReceiveTimeout = 15000,
                    SendTimeout = 15000
                };
                // ConnectAsync é uma operação do próprio Socket; o prazo evita esperar indefinidamente.
                // Task.Run em ServicoApi mantém a interface livre enquanto aguardamos a rede.
                using var timeout = new CancellationTokenSource(TimeSpan.FromSeconds(10));
                socket.ConnectAsync(config.Host, config.Porta, timeout.Token).AsTask().GetAwaiter().GetResult();
                conectado = true;
                Console.WriteLine("[ConexaoServidor] Conectado com sucesso");
            }
            catch (Exception ex)
            {
                socket?.Dispose();
                socket = null!;
                conectado = false;
                Console.WriteLine($"[ConexaoServidor] Erro ao conectar: {ex.Message}");
                // Preserva a exceção original para diagnóstico e mostra ao usuário o destino real.
                string motivo = ex switch
                {
                    SocketException { SocketErrorCode: SocketError.ConnectionRefused } => "Conexão recusada pelo destino",
                    SocketException { SocketErrorCode: SocketError.HostNotFound } => "Nome do servidor não encontrado",
                    OperationCanceledException => "Tempo de conexão esgotado (10 segundos)",
                    _ => ex.Message
                };
                throw new IOException(
                    $"{motivo}: {config.Host}:{config.Porta}.\n\n" +
                    "1. Inicie ServerSide no PC que guarda o banco e aguarde 'Servidor TCP aguardando clientes'.\n" +
                    "2. Em Servidor... na tela de login, informe o IPv4 desse PC (ipconfig).\n" +
                    "3. Confira a porta e a liberação TCP no firewall.\n\n" +
                    "localhost/127.0.0.1 apontam para este computador; use-os somente se o servidor estiver aqui.", ex);
            }
        }

        /// <summary>
        /// Envia uma mensagem para o servidor usando o protocolo padronizado.
        /// </summary>
        public void Enviar(Mensagem mensagem)
        {
            if (!EstaConectado)
                throw new InvalidOperationException("Não conectado ao servidor");

            try
            {
                Console.WriteLine("[ConexaoServidor] Enviando mensagem...");
                Protocolo.Enviar(socket, mensagem);
                Console.WriteLine("[ConexaoServidor] Mensagem enviada");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ConexaoServidor] Erro ao enviar: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Recebe uma mensagem do servidor usando o protocolo padronizado.
        /// </summary>
        public Mensagem Receber()
        {
            if (!EstaConectado)
                throw new InvalidOperationException("Não conectado ao servidor");

            try
            {
                Console.WriteLine("[ConexaoServidor] Recebendo mensagem...");
                // Receive pode retornar fim de conexão: isso é falha, e não um login rejeitado.
                Mensagem resposta = Protocolo.Receber(socket)
                    ?? throw new IOException("O servidor fechou a conexão antes de responder.");
                Console.WriteLine("[ConexaoServidor] Mensagem recebida");
                return resposta;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ConexaoServidor] Erro ao receber: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Desconecta do servidor.
        /// </summary>
        public void Desconectar()
        {
            try
            {
                if (socket != null && socket.Connected)
                {
                    // Encerra envio e recepção antes de liberar o descritor nativo.
                    socket.Shutdown(SocketShutdown.Both);
                }
            }
            catch { }
            finally
            {
                socket?.Close();
                socket?.Dispose();
                conectado = false;
                Console.WriteLine("[ConexaoServidor] Desconectado");
            }
        }

        public void Dispose()
        {
            Desconectar();
        }
    }
}
