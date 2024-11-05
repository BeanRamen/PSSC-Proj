namespace lab3.Models
{
    public class PriceDto
    {
        public int PriceId { get; set; }
        public int CartId { get; set; }
        public decimal? Item { get; set; }
        public decimal? TVA { get; set; }
        public decimal? Final { get; set; }
    }
}