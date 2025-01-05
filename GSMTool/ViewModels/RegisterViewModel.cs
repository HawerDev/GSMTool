using System.ComponentModel.DataAnnotations;

namespace GSMTool.ViewModels
{
    public class RegisterViewModel
    {
        [Required(ErrorMessage = "Nazwa użytkownika jest wymagana")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Nazwa użytkownika musi mieć od 3 do 50 znaków")]
        [Display(Name = "Nazwa użytkownika")]
        public string Username { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email jest wymagany")]
        [EmailAddress(ErrorMessage = "Nieprawidłowy format adresu email")]
        [Display(Name = "Email")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Hasło jest wymagane")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Hasło musi mieć co najmniej 6 znaków")]
        [DataType(DataType.Password)]
        [Display(Name = "Hasło")]
        public string Password { get; set; } = string.Empty;

        [DataType(DataType.Password)]
        [Display(Name = "Potwierdź hasło")]
        [Compare("Password", ErrorMessage = "Hasła nie są identyczne")]
        public string ConfirmPassword { get; set; } = string.Empty;
    }
}