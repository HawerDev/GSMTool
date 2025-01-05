using System.ComponentModel.DataAnnotations;

namespace GSMTool.Models
{
    public class Part
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Nazwa części jest wymagana")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Ilość jest wymagana")]
        [Range(0, int.MaxValue, ErrorMessage = "Ilość nie może być ujemna")]
        public int Quantity { get; set; }

        [Required(ErrorMessage = "Cena jest wymagana")]
        [Range(0, double.MaxValue, ErrorMessage = "Cena nie może być ujemna")]
        [DisplayFormat(DataFormatString = "{0:C}", ApplyFormatInEditMode = false)]
        public decimal Price { get; set; }

        public string? QRCode { get; set; }  
    }
}