using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MuseoMineralogia.Models
{
    public class Carrello
    {
        [Key]
        public int CarrelloId { get; set; }

        [Required]
        public string UtenteId { get; set; } = string.Empty;
        public virtual Utente? Utente { get; set; }

        public virtual ICollection<ElementoCarrello>? ElementiCarrello { get; set; }
    }
}
