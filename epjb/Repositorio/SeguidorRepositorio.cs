using System;
using System.Collections.Generic;
using System.Linq;
using epjb.Data;
using epjb.Models;

namespace epjb.Repositorio
{
    /// <summary>
    /// Repositório para operações de seguir/desseguir usuários.
    /// </summary>
    public class SeguidorRepositorio : IDisposable
    {
        private readonly AppDbContext dbContext;

        public SeguidorRepositorio()
        {
            dbContext = new AppDbContext();
        }

        /// <summary>
        /// Um usuário segue outro usuário.
        /// </summary>
        public bool Seguir(int idUsuario, int idUsuarioASeguir)
        {
            try
            {
                if (idUsuario == idUsuarioASeguir)
                {
                    Console.WriteLine("[SeguidorRepositorio] Usuário não pode seguir a si mesmo");
                    return false;
                }

                // Verifica se já segue
                var jaSegue = dbContext.UsuarioSeguidores
                    .Any(us => us.IdUsuario == idUsuario && us.IdSeguido == idUsuarioASeguir);

                if (jaSegue)
                {
                    Console.WriteLine("[SeguidorRepositorio] Usuário já segue este usuário");
                    return false;
                }

                var seguimento = new UsuarioSeguidor
                {
                    IdUsuario = idUsuario,
                    IdSeguido = idUsuarioASeguir,
                    DataSeguimento = DateTime.Now
                };

                dbContext.UsuarioSeguidores.Add(seguimento);
                dbContext.SaveChanges();

                Console.WriteLine($"[SeguidorRepositorio] Usuário {idUsuario} agora segue {idUsuarioASeguir}");
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[SeguidorRepositorio] Erro ao seguir: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Um usuário para de seguir outro usuário.
        /// </summary>
        public bool Desseguir(int idUsuario, int idUsuarioADesseguir)
        {
            try
            {
                var seguimento = dbContext.UsuarioSeguidores
                    .FirstOrDefault(us => us.IdUsuario == idUsuario && us.IdSeguido == idUsuarioADesseguir);

                if (seguimento == null)
                {
                    Console.WriteLine("[SeguidorRepositorio] Relação de seguimento não encontrada");
                    return false;
                }

                dbContext.UsuarioSeguidores.Remove(seguimento);
                dbContext.SaveChanges();

                Console.WriteLine($"[SeguidorRepositorio] Usuário {idUsuario} deixou de seguir {idUsuarioADesseguir}");
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[SeguidorRepositorio] Erro ao desseguir: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Lista todos os usuários que um usuário está seguindo.
        /// </summary>
        public List<Usuario> ListarSeguindo(int idUsuario)
        {
            try
            {
                var seguindo = dbContext.UsuarioSeguidores
                    .Where(us => us.IdUsuario == idUsuario)
                    .Select(us => us.UsuarioSeguido)
                    .ToList();

                Console.WriteLine($"[SeguidorRepositorio] Usuário {idUsuario} segue {seguindo.Count} usuários");
                return seguindo;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[SeguidorRepositorio] Erro ao listar seguindo: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Lista todos os seguidores de um usuário.
        /// </summary>
        public List<Usuario> ListarSeguidores(int idUsuario)
        {
            try
            {
                var seguidores = dbContext.UsuarioSeguidores
                    .Where(us => us.IdSeguido == idUsuario)
                    .Select(us => us.Usuario)
                    .ToList();

                Console.WriteLine($"[SeguidorRepositorio] Usuário {idUsuario} tem {seguidores.Count} seguidores");
                return seguidores;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[SeguidorRepositorio] Erro ao listar seguidores: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Verifica se um usuário segue outro.
        /// </summary>
        public bool EstaSeguidoPor(int idUsuario, int idOutroUsuario)
        {
            try
            {
                return dbContext.UsuarioSeguidores
                    .Any(us => us.IdUsuario == idOutroUsuario && us.IdSeguido == idUsuario);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[SeguidorRepositorio] Erro ao verificar seguimento: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Obtém o relacionamento de seguimento entre dois usuários.
        /// </summary>
        public UsuarioSeguidor ObterRelacionamento(int idUsuario, int idOutroUsuario)
        {
            try
            {
                return dbContext.UsuarioSeguidores
                    .FirstOrDefault(us => us.IdUsuario == idUsuario && us.IdSeguido == idOutroUsuario);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[SeguidorRepositorio] Erro ao obter relacionamento: {ex.Message}");
                throw;
            }
        }

        public void Dispose()
        {
            dbContext?.Dispose();
        }
    }
}
