using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using epjb.Common;

namespace epjb.Cliente.Rede
{
    /// <summary>
    /// Monta comandos do nosso protocolo TCP; o nome API não significa uma API HTTP.
    /// ConexaoServidor e Protocolo são fontes do grupo com as operações de Socket explícitas.
    /// </summary>
    public class ServicoApi : IDisposable
    {
        /// <summary>
        /// Realiza o login do usuário de forma assíncrona.
        /// </summary>
        public async Task<Mensagem> LoginAsync(string username, string password)
        {
            return await ExecutarComandoAsync(Comando.LOGIN, new { username, password });
        }

        /// <summary>
        /// Realiza o cadastro do usuário de forma assíncrona.
        /// </summary>
        public async Task<Mensagem> CadastroAsync(string username, string password)
        {
            return await ExecutarComandoAsync(Comando.CADASTRO, new { username, password });
        }

        /// <summary>
        /// Lista todas as mensagens da plataforma.
        /// </summary>
        public async Task<Mensagem> ListarMensagensAsync()
        {
            return await ExecutarComandoAsync(Comando.LISTAR_MSGS, new { });
        }

        /// <summary>
        /// Posta uma nova mensagem.
        /// </summary>
        public async Task<Mensagem> PostarMensagemAsync(int idUsuario, string conteudo)
        {
            return await ExecutarComandoAsync(Comando.POSTAR_MSG, new { idUsuario, conteudo });
        }

        /// <summary>
        /// Deleta uma mensagem (apenas próprias mensagens).
        /// </summary>
        public async Task<Mensagem> DeletarMensagemAsync(int idMensagem, int idUsuario)
        {
            return await ExecutarComandoAsync(Comando.DELETAR_MSG, new { idMensagem, idUsuario });
        }

        /// <summary>
        /// Segue um usuário.
        /// </summary>
        public async Task<Mensagem> SeguirUsuarioAsync(int idUsuario, int idUsuarioASeguir)
        {
            return await ExecutarComandoAsync(Comando.SEGUIR, new { idUsuario, idUsuarioASeguir });
        }

        /// <summary>
        /// Lista todos os usuários da plataforma.
        /// </summary>
        public async Task<Mensagem> ListarUsuariosAsync()
        {
            return await ExecutarComandoAsync(Comando.LISTAR_USUARIOS, new { });
        }

        /// <summary>
        /// Lista os seguidores de um usuário.
        /// </summary>
        public async Task<Mensagem> ListarMeusSeguidoresAsync(int idUsuario)
        {
            return await ExecutarComandoAsync(Comando.LISTAR_MEUS_SEGUIDORES, new { idUsuario });
        }

        /// <summary>
        /// Lista os usuários que o usuário está seguindo.
        /// </summary>
        public async Task<Mensagem> ListarMeusSeguindoAsync(int idUsuario)
        {
            return await ExecutarComandoAsync(Comando.LISTAR_MEUS_SEGUINDO, new { idUsuario });
        }

        /// <summary>
        /// Executa um comando genérico de forma assíncrona.
        /// Gerencia a conexão, envio, recebimento e fechamento automaticamente.
        /// </summary>
        private async Task<Mensagem> ExecutarComandoAsync(Comando tipo, object payload)
        {
            // Apenas o agendamento é delegado à Task. Todo o transporte é Socket programado pelo grupo.
            return await Task.Run(() =>
            {
                // Uma conexão por requisição evita misturar respostas de botões acionados simultaneamente.
                using var conexao = new ConexaoServidor();
                try
                {
                    Console.WriteLine($"[ServicoApi] Executando comando: {tipo}");

                    // Conecta ao servidor
                    conexao.Conectar();

                    // Serializa o payload para JSON
                    string json = JsonSerializer.Serialize(payload);

                    // Envia o comando
                    conexao.Enviar(new Mensagem(tipo, json));

                    // Recebe a resposta
                    Mensagem resposta = conexao.Receber();

                    Console.WriteLine($"[ServicoApi] Resposta recebida: Sucesso={resposta?.Sucesso}");

                    return resposta;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[ServicoApi] Erro: {ex.Message}");
                    return Mensagem.RespostaErro(tipo, ex.Message);
                }
            });
        }

        public void Dispose()
        {
            // Cada chamada possui e descarta sua própria conexão.
        }
    }
}
