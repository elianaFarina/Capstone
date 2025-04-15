using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MuseoMineralogia.Models
{
    public class Carrello
    {
        [Key]
        public int CarrelloId { get; set; }

        [Required]
        public int UtenteId { get; set; }
        public virtual Utente? Utente { get; set; }

        public virtual ICollection<ElementoCarrello>? ElementiCarrello { get; set; }
    }
}
