using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace epjb.Models
{
    // Aresta do relacionamento: IdUsuario segue IdSeguido; a direção não é simétrica.
    public class UsuarioSeguidor
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int IdUsuario { get; set; }

        [Required]
        public int IdSeguido { get; set; }

        [Required]
        public DateTime DataSeguimento { get; set; } = DateTime.Now;

        // Relacionamentos
        [ForeignKey(nameof(IdUsuario))]
        public Usuario? Usuario { get; set; }

        [ForeignKey(nameof(IdSeguido))]
        public Usuario? UsuarioSeguido { get; set; }
    }
}