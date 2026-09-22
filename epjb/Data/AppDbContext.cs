using Microsoft.EntityFrameworkCore;
using epjb.Models;

namespace epjb.Data
{
    public class AppDbContext : DbContext
    {
        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Mensagem> Mensagens { get; set; }
        public DbSet<UsuarioSeguidor> UsuarioSeguidores { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            options.UseSqlite("Data Source=epjb.db");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configuração do relacionamento Many-to-Many de seguidores
            modelBuilder.Entity<UsuarioSeguidor>()
                .HasOne(us => us.Usuario)
                .WithMany(u => u.Seguindo)
                .HasForeignKey(us => us.IdUsuario)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<UsuarioSeguidor>()
                .HasOne(us => us.UsuarioSeguido)
                .WithMany(u => u.Seguidores)
                .HasForeignKey(us => us.IdSeguido)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}