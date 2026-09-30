using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace epjb.Models
{
    // Post persistido: IdUsuario identifica o autor, DataEdicao só é preenchida após edição.
    public class Mensagem
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int IdUsuario { get; set; }

        [Required]
        [MaxLength(500)]
        public string Conteudo { get; set; } = string.Empty;

        [Required]
        public DateTime DataCriacao { get; set; } = DateTime.Now;

        public DateTime? DataEdicao { get; set; }

        // Relacionamento
        [ForeignKey(nameof(IdUsuario))]
        public Usuario? Usuario { get; set; }
    }
}