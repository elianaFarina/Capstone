using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
namespace MuseoMineralogia.Models
{
    public class Ordine
    {
        [Key]
        public int OrdineId { get; set; }

        [Required]
        public string UtenteId { get; set; } = string.Empty;

        public virtual Utente? Utente { get; set; }

        [Required]
        public DateTime DataOrdine { get; set; }

        [StringLength(50)]
        public string? Stato { get; set; }

        [StringLength(100)]
        public string? SessionId { get; set; }

        [StringLength(100)]
        public string? PaymentIntentId { get; set; }

        public virtual ICollection<DettaglioOrdine>? DettagliOrdine { get; set; }
    }
}