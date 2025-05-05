using System.ComponentModel.DataAnnotations;

namespace MuseoMineralogia.Models
{
    public class TipoBiglietto
    {
        [Key]
        public int TipoBigliettoId { get; set; }

        [Required]
        [StringLength(50)]
        public string Nome { get; set; } = string.Empty;

        public decimal Prezzo { get; set; }
    }
}