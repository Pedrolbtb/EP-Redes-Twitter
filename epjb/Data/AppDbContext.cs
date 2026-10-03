using Microsoft.EntityFrameworkCore;
using epjb.Models;

namespace epjb.Data
{
    // Entity Framework cuida apenas da persistência local no servidor; não transporta mensagens de rede.
    public class AppDbContext : DbContext
    {
        // Cada DbSet corresponde a uma tabela; o contexto pertence a uma operação/repositório.
        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Mensagem> Mensagens { get; set; }
        public DbSet<UsuarioSeguidor> UsuarioSeguidores { get; set; }

        // Caminho estável, independente do diretório de onde o servidor foi iniciado.
        public static string CaminhoBanco => Path.GetFullPath(
            Environment.GetEnvironmentVariable("EPJB_DB_PATH")
            ?? Path.Combine(AppContext.BaseDirectory, "dados", "epjb.db"));

        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            // O caminho é mostrado no console para evitar que dois servidores usem bancos diferentes sem perceber.
            Directory.CreateDirectory(Path.GetDirectoryName(CaminhoBanco)!);
            var connectionString = new Microsoft.Data.Sqlite.SqliteConnectionStringBuilder
            {
                DataSource = CaminhoBanco,
                // SQLite serializa escritas: aguarda a liberação de uma escrita concorrente.
                DefaultTimeout = 30
            };
            options.UseSqlite(connectionString.ToString());
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