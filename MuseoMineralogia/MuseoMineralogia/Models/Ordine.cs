namespace MuseoMineralogia.Models
{
    public class Ordine
    {
        public int Id { get; set; }
        public DateTime DataOrdine { get; set; }
        public int UtenteId { get; set; }
        public Utente? Utente { get; set; }
        public decimal Totale { get; set; }
    }
}
