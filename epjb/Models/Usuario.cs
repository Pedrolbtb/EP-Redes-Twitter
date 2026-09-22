using System.ComponentModel.DataAnnotations;

namespace epjb.Models
{
    public class Usuario
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Username { get; set; } = string.Empty;

        [Required]
        [MaxLength(255)]
        public string Senha { get; set; } = string.Empty;

        [Required]
        public DateTime DataCriacao { get; set; } = DateTime.Now;

        // Relacionamentos
        public ICollection<Mensagem> Mensagens { get; set; } = new List<Mensagem>();
        public ICollection<UsuarioSeguidor> Seguindo { get; set; } = new List<UsuarioSeguidor>();
        public ICollection<UsuarioSeguidor> Seguidores { get; set; } = new List<UsuarioSeguidor>();
    }
}