using System.ComponentModel.DataAnnotations;

public class ForgotPasswordViewModel
{
    [Required(ErrorMessage = "L'indirizzo email è obbligatorio")]
    [EmailAddress(ErrorMessage = "Indirizzo email non valido")]
    public string Email { get; set; } = string.Empty;
}
