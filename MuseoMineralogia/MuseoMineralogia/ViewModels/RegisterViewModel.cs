using System.ComponentModel.DataAnnotations;

namespace MuseoMineralogia.ViewModels
{
    public class RegisterViewModel
    {
        [Required(ErrorMessage = "Il campo Email è obbligatorio")]
        [EmailAddress(ErrorMessage = "Formato email non valido")]
        [Display(Name = "Email")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Il campo Password è obbligatorio")]
        [StringLength(100, ErrorMessage = "La {0} deve essere di almeno {2} caratteri.", MinimumLength = 8)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; } = string.Empty;

        [DataType(DataType.Password)]
        [Display(Name = "Conferma password")]
        [Compare("Password", ErrorMessage = "Le password non corrispondono.")]
        public string ConfirmPassword { get; set; } = string.Empty;

        [Required(ErrorMessage = "Il campo Nome è obbligatorio")]
        [Display(Name = "Nome")]
        public string Nome { get; set; } = string.Empty;

        [Required(ErrorMessage = "Il campo Cognome è obbligatorio")]
        [Display(Name = "Cognome")]
        public string Cognome { get; set; } = string.Empty;
    }
}