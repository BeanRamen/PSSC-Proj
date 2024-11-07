using System.ComponentModel.DataAnnotations;
using lab3.Models;

namespace API.Models
{
    public class InputPrice
    {
        [Required]
        //[RegularExpression(CartRegistrationNumbe)]
        public string RegistrationNumber { get; set; } = string.Empty;

        [Required]
        [Range(1, 10)]
        public string Item { get; set; }

        [Required]
        [Range(1, 10)]
        public string TVA { get; set; }
    }
}