using System.ComponentModel.DataAnnotations;
namespace MuseoMineralogia.Models
{
    public class ForgotPasswordModel
    {
        [Required(ErrorMessage = "L'email è obbligatoria")]
        [EmailAddress(ErrorMessage = "Formato email non valido")]
        [Display(Name = "Email")]
        public string Email { get; set; } = string.Empty;
    }
}