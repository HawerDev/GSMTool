using System.ComponentModel.DataAnnotations;
namespace GSMTool.Models
{
    public class Device
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Model jest wymagany")]
        public string Model { get; set; } = string.Empty;

        [Required(ErrorMessage = "Numer IMEI jest wymagany")]
        [StringLength(15, MinimumLength = 15, ErrorMessage = "Numer IMEI musi mieć dokładnie 15 znaków")]
        public string IMEINumber { get; set; } = string.Empty;

        [Required]
        public string Status { get; set; } = "Nowe";

        public string? Description { get; set; }

        [Required(ErrorMessage = "Nazwa klienta jest wymagana")]
        public string CustomerName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Numer telefonu jest wymagany")]
        [Phone(ErrorMessage = "Nieprawidłowy format numeru telefonu")]
        public string CustomerPhone { get; set; } = string.Empty;

        public DateTime CreatedDate { get; set; } = DateTime.Now;

        public string ServiceNumber { get; set; } = string.Empty;
    }
}