
using System.ComponentModel.DataAnnotations;

namespace MuseoMineralogia.Models
{
    public class Carrello
    {
        public int CarrelloId { get; set; }
        public string UtenteId { get; set; } = string.Empty;
        public virtual Utente? Utente { get; set; }
        public virtual ICollection<ElementoCarrello>? ElementiCarrello { get; set; }

        public decimal CalcolaTotale()
        {
            if (ElementiCarrello == null || !ElementiCarrello.Any())
                return 0;

            return ElementiCarrello.Sum(e => e.TipoBiglietto?.Prezzo * e.Quantita ?? 0);
        }
    }
}
