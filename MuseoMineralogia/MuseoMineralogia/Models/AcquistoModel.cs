using System.ComponentModel.DataAnnotations;

namespace MuseoMineralogia.Models
{
    public class AcquistoModel
    {
        [Required]
        public int BigliettoId { get; set; }

        [Required]
        [Range(1, 10, ErrorMessage = "La quantità deve essere compresa tra 1 e 10")]
        public int Quantita { get; set; }
    }
}