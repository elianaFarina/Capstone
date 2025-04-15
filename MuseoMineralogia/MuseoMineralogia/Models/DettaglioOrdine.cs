using System.ComponentModel.DataAnnotations;

namespace MuseoMineralogia.Models
{

    public class DettaglioOrdine
    {
        [Key]
        public int DettaglioOrdineId { get; set; }

        [Required]
        public int OrdineId { get; set; }
        public virtual Ordine? Ordine { get; set; }

        [Required]
        public int TipoBigliettoId { get; set; }
        public virtual TipoBiglietto? TipoBiglietto { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "La quantità deve essere almeno 1.")]
        public int Quantita { get; set; }

        public decimal PrezzoUnitario { get; set; }
    }
}