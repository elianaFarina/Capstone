using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;

namespace MuseoMineralogia.Models
{
    public class Utente : IdentityUser
    {
        public string Nome { get; set; } = string.Empty;
        public string Cognome { get; set; } = string.Empty;
        public virtual ICollection<Ordine>? Ordini { get; set; }
        public virtual Carrello? Carrello { get; set; }
    }
}