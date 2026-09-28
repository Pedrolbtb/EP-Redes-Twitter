using System;
using System.Net;
using System.Net.Sockets;
using epjb.Common;

namespace epjb.Cliente.Rede
{
    /// <summary>
    /// Responsável pela conexão TCP com o servidor.
    /// Encapsula a lógica de conectar e comunicar usando o protocolo padronizado.
    /// </summary>
    public class ConexaoServidor : IDisposable
    {
        private Socket socket = null!;
        private bool conectado = false;

        public bool EstaConectado => conectado && socket?.Connected == true;

        /// <summary>
        /// Conecta ao endereço configurado em servidor.json ou EPJB_SERVER_HOST.
        /// </summary>
        public void Conectar()
        {
            try
            {
                Console.WriteLine("[ConexaoServidor] Iniciando conexão...");
                var config = ConfiguracaoServidor.Carregar();
                socket = new Socket(SocketType.Stream, ProtocolType.Tcp)
                {
                    ReceiveTimeout = 15000,
                    SendTimeout = 15000
                };
                using var timeout = new CancellationTokenSource(TimeSpan.FromSeconds(10));
                socket.ConnectAsync(config.Host, config.Porta, timeout.Token).AsTask().GetAwaiter().GetResult();
                conectado = true;
                Console.WriteLine("[ConexaoServidor] Conectado com sucesso");
            }
            catch (Exception ex)
            {
                socket?.Dispose();
                conectado = false;
                Console.WriteLine($"[ConexaoServidor] Erro ao conectar: {ex.Message}");
                throw;
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
                Mensagem resposta = Protocolo.Receber(socket);
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
