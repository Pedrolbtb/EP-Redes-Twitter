using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using epjb.Data;
using epjb.Models;

namespace epjb.Repositorio
{
    /// <summary>
    /// Repositório para operações com mensagens.
    /// </summary>
    public class MensagemRepositorio : IDisposable
    {
        private readonly AppDbContext dbContext;

        public MensagemRepositorio()
        {
            dbContext = new AppDbContext();
        }

        /// <summary>
        /// Lista todas as mensagens da plataforma com informações do autor.
        /// </summary>
        public List<Mensagem> ListarTodas()
        {
            try
            {
                var mensagens = dbContext.Mensagens
                    .OrderByDescending(m => m.DataCriacao)
                    .Include("Usuario")
                    .ToList();

                Console.WriteLine($"[MensagemRepositorio] Listadas {mensagens.Count} mensagens");
                return mensagens;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[MensagemRepositorio] Erro ao listar mensagens: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Lista mensagens de um usuário específico.
        /// </summary>
        public List<Mensagem> ListarPorUsuario(int idUsuario)
        {
            try
            {
                var mensagens = dbContext.Mensagens
                    .Where(m => m.IdUsuario == idUsuario)
                    .OrderByDescending(m => m.DataCriacao)
                    .ToList();

                Console.WriteLine($"[MensagemRepositorio] Listadas {mensagens.Count} mensagens do usuário {idUsuario}");
                return mensagens;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[MensagemRepositorio] Erro ao listar mensagens por usuário: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Cria uma nova mensagem.
        /// </summary>
        public Mensagem Criar(int idUsuario, string conteudo)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(conteudo))
                {
                    Console.WriteLine("[MensagemRepositorio] Conteúdo vazio");
                    return null;
                }

                if (conteudo.Length > 500)
                {
                    Console.WriteLine("[MensagemRepositorio] Conteúdo excede limite de 500 caracteres");
                    return null;
                }

                var mensagem = new Mensagem
                {
                    IdUsuario = idUsuario,
                    Conteudo = conteudo,
                    DataCriacao = DateTime.Now
                };

                dbContext.Mensagens.Add(mensagem);
                dbContext.SaveChanges();

                Console.WriteLine($"[MensagemRepositorio] Mensagem criada com ID {mensagem.Id}");
                return mensagem;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[MensagemRepositorio] Erro ao criar mensagem: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Edita uma mensagem existente (apenas o proprietário pode editar).
        /// </summary>
        public bool Editar(int idMensagem, int idUsuario, string novoConteudo)
        {
            try
            {
                var mensagem = dbContext.Mensagens.Find(idMensagem);
                if (mensagem == null)
                {
                    Console.WriteLine("[MensagemRepositorio] Mensagem não encontrada");
                    return false;
                }

                if (mensagem.IdUsuario != idUsuario)
                {
                    Console.WriteLine($"[MensagemRepositorio] Usuário {idUsuario} não é proprietário da mensagem");
                    return false;
                }

                if (string.IsNullOrWhiteSpace(novoConteudo) || novoConteudo.Length > 500)
                {
                    Console.WriteLine("[MensagemRepositorio] Conteúdo inválido");
                    return false;
                }

                mensagem.Conteudo = novoConteudo;
                mensagem.DataEdicao = DateTime.Now;
                dbContext.SaveChanges();

                Console.WriteLine($"[MensagemRepositorio] Mensagem {idMensagem} editada");
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[MensagemRepositorio] Erro ao editar mensagem: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Deleta uma mensagem (apenas o proprietário pode deletar).
        /// </summary>
        public bool Deletar(int idMensagem, int idUsuario)
        {
            try
            {
                var mensagem = dbContext.Mensagens.Find(idMensagem);
                if (mensagem == null)
                {
                    Console.WriteLine("[MensagemRepositorio] Mensagem não encontrada");
                    return false;
                }

                if (mensagem.IdUsuario != idUsuario)
                {
                    Console.WriteLine($"[MensagemRepositorio] Usuário {idUsuario} não é proprietário da mensagem");
                    return false;
                }

                dbContext.Mensagens.Remove(mensagem);
                dbContext.SaveChanges();

                Console.WriteLine($"[MensagemRepositorio] Mensagem {idMensagem} deletada");
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[MensagemRepositorio] Erro ao deletar mensagem: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Obtém uma mensagem por ID.
        /// </summary>
        public Mensagem ObterPorId(int idMensagem)
        {
            try
            {
                return dbContext.Mensagens.Find(idMensagem);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[MensagemRepositorio] Erro ao obter mensagem: {ex.Message}");
                throw;
            }
        }

        public void Dispose()
        {
            dbContext?.Dispose();
        }
    }
}
