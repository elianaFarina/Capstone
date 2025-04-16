using System.ComponentModel.DataAnnotations;

namespace MuseoMineralogia.ViewModels
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Il campo Email è obbligatorio")]
        [EmailAddress(ErrorMessage = "Formato email non valido")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Il campo Password è obbligatorio")]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;

        [Display(Name = "Ricordami")]
        public bool RememberMe { get; set; }
    }
}
