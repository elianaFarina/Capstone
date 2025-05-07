
using System.ComponentModel.DataAnnotations;

namespace MuseoMineralogia.Models
{
    public class ElementoCarrello
    {
        [Key]
        public int ElementoCarrelloId { get; set; }

        [Required]
        public int CarrelloId { get; set; }

        public virtual Carrello? Carrello { get; set; }

        [Required]
        public int TipoBigliettoId { get; set; }

        public virtual TipoBiglietto? TipoBiglietto { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "La quantità deve essere almeno 1.")]
        public int Quantita { get; set; }
    }
}