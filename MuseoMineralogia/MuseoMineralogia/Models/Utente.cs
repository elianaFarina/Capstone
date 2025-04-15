using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MuseoMineralogia.Models
{
    // Utente per autenticazione
    public class Utente
    {
        [Key]
        public int UtenteId { get; set; }

        [Required]
        [StringLength(100)]
        public string? Username { get; set; }

        [Required]
        public string? PasswordHash { get; set; }

        [Required]
        [StringLength(100)]
        [EmailAddress]
        public string? Email { get; set; }

        [StringLength(100)]
        public string? Nome { get; set; }

        [StringLength(100)]
        public string? Cognome { get; set; }

        public virtual ICollection<Ordine>? Ordini { get; set; }

        public virtual Carrello? Carrello { get; set; }
    }
}
