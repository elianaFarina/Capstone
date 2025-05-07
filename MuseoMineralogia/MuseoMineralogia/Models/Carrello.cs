
using System.ComponentModel.DataAnnotations;

namespace MuseoMineralogia.Models
{
    // Verifica che il modello Carrello sia simile a questo
    public class Carrello
    {
        public int CarrelloId { get; set; }
        public string UtenteId { get; set; } = string.Empty;
        public virtual Utente? Utente { get; set; }
        public virtual ICollection<ElementoCarrello>? ElementiCarrello { get; set; }

        // Aggiungi un metodo per calcolare il totale
        public decimal CalcolaTotale()
        {
            if (ElementiCarrello == null || !ElementiCarrello.Any())
                return 0;

            return ElementiCarrello.Sum(e => e.TipoBiglietto?.Prezzo * e.Quantita ?? 0);
        }
    }
}
