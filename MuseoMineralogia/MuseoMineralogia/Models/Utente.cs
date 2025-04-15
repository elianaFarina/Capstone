namespace MuseoMineralogia.Models
{
    public class Utente
    {
        public int? Id { get; set; }
        public string? Nome { get; set; }
        public string? Email { get; set; }
        public string? PasswordHash { get; set; }
    }
}
