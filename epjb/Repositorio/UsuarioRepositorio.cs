using System;
using System.Collections.Generic;
using System.Linq;
using epjb.Data;
using epjb.Models;

namespace epjb.Repositorio
{
    /// <summary>
    /// Repositório de usuários com operações de autenticação e gerenciamento.
    /// </summary>
    public class UsuarioRepositorio : IDisposable
    {
        private readonly AppDbContext dbContext;

        public UsuarioRepositorio()
        {
            dbContext = new AppDbContext();
        }

        /// <summary>
        /// Autentica um usuário verificando username e senha no banco de dados.
        /// </summary>
        /// <param name="username">Nome de usuário</param>
        /// <param name="senha">Senha do usuário</param>
        /// <returns>Objeto Usuario se autenticado, null caso contrário</returns>
        public Usuario Autenticar(string username, string senha)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(senha))
                {
                    Console.WriteLine("[UsuarioRepositorio] Username ou senha vazios");
                    return null;
                }

                var usuario = dbContext.Usuarios
                    .FirstOrDefault(u => u.Username.ToLower() == username.ToLower() && u.Senha == senha);

                if (usuario != null)
                {
                    Console.WriteLine($"[UsuarioRepositorio] Usuário {username} autenticado com sucesso");
                }
                else
                {
                    Console.WriteLine($"[UsuarioRepositorio] Falha na autenticação de {username}");
                }

                return usuario;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[UsuarioRepositorio] Erro ao autenticar: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Verifica se um username já existe no banco.
        /// </summary>
        public bool UsuarioExiste(string username)
        {
            try
            {
                return dbContext.Usuarios.Any(u => u.Username.ToLower() == username.ToLower());
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[UsuarioRepositorio] Erro ao verificar existência: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Registra um novo usuário no banco.
        /// </summary>
        public Usuario Registrar(string username, string senha)
        {
            try
            {
                if (UsuarioExiste(username))
                {
                    Console.WriteLine($"[UsuarioRepositorio] Usuário {username} já existe");
                    return null;
                }

                var usuario = new Usuario
                {
                    Username = username,
                    Senha = senha,
                    DataCriacao = DateTime.Now
                };

                dbContext.Usuarios.Add(usuario);
                dbContext.SaveChanges();

                Console.WriteLine($"[UsuarioRepositorio] Usuário {username} registrado com sucesso");
                return usuario;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[UsuarioRepositorio] Erro ao registrar: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Obtém um usuário por ID.
        /// </summary>
        public Usuario ObterPorId(int id)
        {
            try
            {
                return dbContext.Usuarios.Find(id);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[UsuarioRepositorio] Erro ao obter usuário: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Lista todos os usuários da plataforma.
        /// </summary>
        public List<Usuario> ListarTodos()
        {
            try
            {
                var usuarios = dbContext.Usuarios.ToList();
                Console.WriteLine($"[UsuarioRepositorio] Listados {usuarios.Count} usuários");
                return usuarios;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[UsuarioRepositorio] Erro ao listar usuários: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Obtém um usuário por username.
        /// </summary>
        public Usuario ObterPorUsername(string username)
        {
            try
            {
                return dbContext.Usuarios
                    .FirstOrDefault(u => u.Username.ToLower() == username.ToLower());
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[UsuarioRepositorio] Erro ao obter usuário por username: {ex.Message}");
                throw;
            }
        }

        public void Dispose()
        {
            dbContext?.Dispose();
        }
    }
}

