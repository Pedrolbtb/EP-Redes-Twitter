using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using epjb.Common;
using epjb.Repositorio;

namespace epjb.Sockets
{
    // Listener do grupo: não utiliza TcpListener, ASP.NET, SignalR nem RPC.
    public class AsyncSocketListener
    {
        public static void StartListener()
        {
            // IPv4 + Stream + Tcp cria o descritor de escuta diretamente no sistema operacional.
            using var listener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            // Any = 0.0.0.0: aceita conexões locais e de outros PCs, conforme as regras do firewall.
            listener.Bind(new IPEndPoint(IPAddress.Any, Config.Porta));
            // A fila comporta até 100 conexões aguardando Accept; ainda não há atendimento aqui.
            listener.Listen(100);
            Console.WriteLine($"Servidor TCP aguardando clientes em 0.0.0.0:{Config.Porta}");
            while (true)
            {
                // Accept bloqueia até chegar um cliente e devolve OUTRO socket, exclusivo dessa conexão.
                Socket handler = listener.Accept();
                handler.ReceiveTimeout = 15000;
                handler.SendTimeout = 15000;
                // A próxima conexão pode ser aceita enquanto esta tarefa recebe/processa/envia.
                Task.Run(() => HandleClient(handler));
            }
        }

        // Cada tarefa atende somente seu socket. Os repositórios também têm contextos separados.
        private static void HandleClient(Socket handler)
        {
            try
            {
                while (true)
                {
                    Mensagem? msg = Protocolo.Receber(handler);
                    if (msg == null) break; // cliente desconectou
                    Console.WriteLine($"Recebido comando: {msg.Tipo}");

                    // Processa o comando
                    Mensagem resposta = ProcessarComando(msg);

                    // Envia resposta
                    Protocolo.Enviar(handler, resposta);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro no cliente: {ex}");
            }
            finally
            {
                // Mesmo quando o JSON ou o banco falha, o descritor do cliente é liberado.
                try { handler.Shutdown(SocketShutdown.Both); } catch { }
                try { handler.Close(); } catch { }
            }
        }

        /// <summary>
        /// Processa o comando recebido do cliente e retorna a resposta apropriada.
        /// </summary>
        private static Mensagem ProcessarComando(Mensagem msg)
        {
            try
            {
                // Dispatcher explícito: o enum recebido seleciona o método de negócio escrito pelo grupo.
                switch (msg.Tipo)
                {
                    case Comando.LOGIN:
                        return ProcessarLogin(msg);

                    case Comando.CADASTRO:
                        return ProcessarCadastro(msg);

                    case Comando.LISTAR_MSGS:
                        return ProcessarListarMensagens(msg);

                    case Comando.POSTAR_MSG:
                        return ProcessarPostarMensagem(msg);

                    case Comando.DELETAR_MSG:
                        return ProcessarDeletarMensagem(msg);

                    case Comando.SEGUIR:
                        return ProcessarSeguir(msg);

                    case Comando.LISTAR_USUARIOS:
                        return ProcessarListarUsuarios(msg);

                    case Comando.LISTAR_MEUS_SEGUIDORES:
                        return ProcessarListarMeusSeguidores(msg);

                    case Comando.LISTAR_MEUS_SEGUINDO:
                        return ProcessarListarMeusSeguindo(msg);

                    default:
                        return Mensagem.RespostaErro(msg.Tipo, "Comando não suportado");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Servidor] Erro ao processar comando: {ex.Message}");
                return Mensagem.RespostaErro(msg.Tipo, $"Erro no servidor: {ex.Message}");
            }
        }

        /// <summary>
        /// Processa o comando LOGIN.
        /// </summary>
        private static Mensagem ProcessarLogin(Mensagem msg)
        {
            try
            {
                // Desserializa o payload JSON
                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                var loginRequest = JsonSerializer.Deserialize<LoginRequest>(msg.PayloadJson, options);

                if (loginRequest == null || string.IsNullOrWhiteSpace(loginRequest.Username) || string.IsNullOrWhiteSpace(loginRequest.Password))
                {
                    return Mensagem.RespostaErro(Comando.LOGIN, "Username ou password inválidos");
                }

                // Valida no banco de dados
                using var repositorio = new UsuarioRepositorio();
                var usuario = repositorio.Autenticar(loginRequest.Username, loginRequest.Password);

                if (usuario != null)
                {
                    // Sucesso: retorna dados do usuário
                    var responsePayload = JsonSerializer.Serialize(new { id = usuario.Id, username = usuario.Username });
                    return Mensagem.RespostaSucesso(Comando.LOGIN, responsePayload);
                }
                else
                {
                    return Mensagem.RespostaErro(Comando.LOGIN, "Credenciais inválidas");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Servidor] Erro no login: {ex.Message}");
                return Mensagem.RespostaErro(Comando.LOGIN, ex.Message);
            }
        }

        /// <summary>
        /// Processa o comando CADASTRO.
        /// </summary>
        private static Mensagem ProcessarCadastro(Mensagem msg)
        {
            try
            {
                // Desserializa o payload JSON
                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                var cadastroRequest = JsonSerializer.Deserialize<LoginRequest>(msg.PayloadJson, options);

                if (cadastroRequest == null || string.IsNullOrWhiteSpace(cadastroRequest.Username) || string.IsNullOrWhiteSpace(cadastroRequest.Password))
                {
                    return Mensagem.RespostaErro(Comando.CADASTRO, "Username ou password inválidos");
                }

                // Registra novo usuário
                using var repositorio = new UsuarioRepositorio();
                var usuario = repositorio.Registrar(cadastroRequest.Username, cadastroRequest.Password);

                if (usuario != null)
                {
                    var responsePayload = JsonSerializer.Serialize(new { id = usuario.Id, username = usuario.Username });
                    return Mensagem.RespostaSucesso(Comando.CADASTRO, responsePayload);
                }
                else
                {
                    return Mensagem.RespostaErro(Comando.CADASTRO, "Username já existe");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Servidor] Erro no cadastro: {ex.Message}");
                return Mensagem.RespostaErro(Comando.CADASTRO, ex.Message);
            }
        }

        /// <summary>
        /// Processa o comando LISTAR_MSGS.
        /// </summary>
        private static Mensagem ProcessarListarMensagens(Mensagem msg)
        {
            try
            {
                using var repoMensagem = new MensagemRepositorio();
                var mensagens = repoMensagem.ListarTodas();

                // Projeta somente campos públicos: não envia senha nem entidades com referências circulares.
                var response = JsonSerializer.Serialize(mensagens.Select(m => new
                {
                    id = m.Id,
                    idUsuario = m.IdUsuario,
                    username = m.Usuario?.Username ?? "Desconhecido",
                    conteudo = m.Conteudo,
                    dataCriacao = m.DataCriacao,
                    dataEdicao = m.DataEdicao
                }).ToList());

                return Mensagem.RespostaSucesso(Comando.LISTAR_MSGS, response);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Servidor] Erro ao listar mensagens: {ex.Message}");
                return Mensagem.RespostaErro(Comando.LISTAR_MSGS, ex.Message);
            }
        }

        /// <summary>
        /// Processa o comando POSTAR_MSG.
        /// </summary>
        private static Mensagem ProcessarPostarMensagem(Mensagem msg)
        {
            try
            {
                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                // Extrai os campos do comando do JSON; nenhuma biblioteca despacha chamadas remotamente.
                JsonElement payload = JsonSerializer.Deserialize<JsonElement>(msg.PayloadJson, options);

                if (payload.ValueKind != JsonValueKind.Object)
                    return Mensagem.RespostaErro(Comando.POSTAR_MSG, "Payload inválido");

                int idUsuario = (int)payload.GetProperty("idUsuario").GetInt32();
                string conteudo = payload.GetProperty("conteudo").GetString();

                using var repoMensagem = new MensagemRepositorio();
                var mensagem = repoMensagem.Criar(idUsuario, conteudo);

                if (mensagem != null)
                {
                    // Projeta somente campos públicos: não envia senha nem entidades com referências circulares.
                var response = JsonSerializer.Serialize(new { id = mensagem.Id });
                    return Mensagem.RespostaSucesso(Comando.POSTAR_MSG, response);
                }
                else
                {
                    return Mensagem.RespostaErro(Comando.POSTAR_MSG, "Falha ao criar mensagem");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Servidor] Erro ao postar mensagem: {ex.Message}");
                return Mensagem.RespostaErro(Comando.POSTAR_MSG, ex.Message);
            }
        }

        /// <summary>
        /// Processa o comando DELETAR_MSG.
        /// </summary>
        private static Mensagem ProcessarDeletarMensagem(Mensagem msg)
        {
            try
            {
                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                // Extrai os campos do comando do JSON; nenhuma biblioteca despacha chamadas remotamente.
                JsonElement payload = JsonSerializer.Deserialize<JsonElement>(msg.PayloadJson, options);

                if (payload.ValueKind != JsonValueKind.Object)
                    return Mensagem.RespostaErro(Comando.DELETAR_MSG, "Payload inválido");

                int idMensagem = (int)payload.GetProperty("idMensagem").GetInt32();
                int idUsuario = (int)payload.GetProperty("idUsuario").GetInt32();

                using var repoMensagem = new MensagemRepositorio();
                bool deletado = repoMensagem.Deletar(idMensagem, idUsuario);

                if (deletado)
                {
                    return Mensagem.RespostaSucesso(Comando.DELETAR_MSG, null);
                }
                else
                {
                    return Mensagem.RespostaErro(Comando.DELETAR_MSG, "Não é possível deletar essa mensagem");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Servidor] Erro ao deletar mensagem: {ex.Message}");
                return Mensagem.RespostaErro(Comando.DELETAR_MSG, ex.Message);
            }
        }

        /// <summary>
        /// Processa o comando SEGUIR.
        /// </summary>
        private static Mensagem ProcessarSeguir(Mensagem msg)
        {
            try
            {
                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                // Extrai os campos do comando do JSON; nenhuma biblioteca despacha chamadas remotamente.
                JsonElement payload = JsonSerializer.Deserialize<JsonElement>(msg.PayloadJson, options);

                if (payload.ValueKind != JsonValueKind.Object)
                    return Mensagem.RespostaErro(Comando.SEGUIR, "Payload inválido");

                int idUsuario = (int)payload.GetProperty("idUsuario").GetInt32();
                int idUsuarioASeguir = (int)payload.GetProperty("idUsuarioASeguir").GetInt32();

                using var repoSeguidor = new SeguidorRepositorio();
                bool sucesso = repoSeguidor.Seguir(idUsuario, idUsuarioASeguir);

                if (sucesso)
                {
                    return Mensagem.RespostaSucesso(Comando.SEGUIR, null);
                }
                else
                {
                    return Mensagem.RespostaErro(Comando.SEGUIR, "Não é possível seguir este usuário");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Servidor] Erro ao seguir: {ex.Message}");
                return Mensagem.RespostaErro(Comando.SEGUIR, ex.Message);
            }
        }

        /// <summary>
        /// Processa o comando LISTAR_USUARIOS.
        /// </summary>
        private static Mensagem ProcessarListarUsuarios(Mensagem msg)
        {
            try
            {
                using var repoUsuario = new UsuarioRepositorio();
                var usuarios = repoUsuario.ListarTodos();

                // Projeta somente campos públicos: não envia senha nem entidades com referências circulares.
                var response = JsonSerializer.Serialize(usuarios.Select(u => new
                {
                    id = u.Id,
                    username = u.Username,
                    dataCriacao = u.DataCriacao
                }).ToList());

                return Mensagem.RespostaSucesso(Comando.LISTAR_USUARIOS, response);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Servidor] Erro ao listar usuários: {ex.Message}");
                return Mensagem.RespostaErro(Comando.LISTAR_USUARIOS, ex.Message);
            }
        }

        /// <summary>
        /// Processa o comando LISTAR_MEUS_SEGUIDORES.
        /// </summary>
        private static Mensagem ProcessarListarMeusSeguidores(Mensagem msg)
        {
            try
            {
                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                // Extrai os campos do comando do JSON; nenhuma biblioteca despacha chamadas remotamente.
                JsonElement payload = JsonSerializer.Deserialize<JsonElement>(msg.PayloadJson, options);

                if (payload.ValueKind != JsonValueKind.Object)
                    return Mensagem.RespostaErro(Comando.LISTAR_MEUS_SEGUIDORES, "Payload inválido");

                int idUsuario = (int)payload.GetProperty("idUsuario").GetInt32();

                using var repoSeguidor = new SeguidorRepositorio();
                var seguidores = repoSeguidor.ListarSeguidores(idUsuario);

                // Projeta somente campos públicos: não envia senha nem entidades com referências circulares.
                var response = JsonSerializer.Serialize(seguidores.Select(u => new
                {
                    id = u.Id,
                    username = u.Username
                }).ToList());

                return Mensagem.RespostaSucesso(Comando.LISTAR_MEUS_SEGUIDORES, response);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Servidor] Erro ao listar seguidores: {ex.Message}");
                return Mensagem.RespostaErro(Comando.LISTAR_MEUS_SEGUIDORES, ex.Message);
            }
        }

        /// <summary>
        /// Processa o comando LISTAR_MEUS_SEGUINDO.
        /// </summary>
        private static Mensagem ProcessarListarMeusSeguindo(Mensagem msg)
        {
            try
            {
                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                // Extrai os campos do comando do JSON; nenhuma biblioteca despacha chamadas remotamente.
                JsonElement payload = JsonSerializer.Deserialize<JsonElement>(msg.PayloadJson, options);

                if (payload.ValueKind != JsonValueKind.Object)
                    return Mensagem.RespostaErro(Comando.LISTAR_MEUS_SEGUINDO, "Payload inválido");

                int idUsuario = (int)payload.GetProperty("idUsuario").GetInt32();

                using var repoSeguidor = new SeguidorRepositorio();
                var seguindo = repoSeguidor.ListarSeguindo(idUsuario);

                // Projeta somente campos públicos: não envia senha nem entidades com referências circulares.
                var response = JsonSerializer.Serialize(seguindo.Select(u => new
                {
                    id = u.Id,
                    username = u.Username
                }).ToList());

                return Mensagem.RespostaSucesso(Comando.LISTAR_MEUS_SEGUINDO, response);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Servidor] Erro ao listar seguindo: {ex.Message}");
                return Mensagem.RespostaErro(Comando.LISTAR_MEUS_SEGUINDO, ex.Message);
            }
        }
    }
}

