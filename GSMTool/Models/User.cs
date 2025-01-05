using System.ComponentModel.DataAnnotations;

namespace GSMTool.Models
{
    public class User
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Nazwa użytkownika jest wymagana")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Nazwa użytkownika musi mieć od 3 do 50 znaków")]
        public string Username { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email jest wymagany")]
        [EmailAddress(ErrorMessage = "Nieprawidłowy format adresu email")]
        public string Email { get; set; } = string.Empty;

        public string PasswordHash { get; set; } = string.Empty;

        [Required(ErrorMessage = "Rola jest wymagana")]
        public string Role { get; set; } = "User";
    }
}