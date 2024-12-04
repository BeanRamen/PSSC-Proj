using System.ComponentModel.DataAnnotations;
using lab3.Models;

namespace API.Models
{
    public class InputPrice
    {
        public InputPrice(string item, string tva)
        {
            Item = item;
            TVA = tva;
        }

        [Required]
        //[RegularExpression(CartRegistrationNumbe)]
        public string RegistrationNumber { get; set; } = string.Empty;

        [Required]
        [Range(1, 10000)]
        public string Item { get; set; }

        [Required]
        [Range(0, 10000)]
        public string TVA { get; set; }
    }
}