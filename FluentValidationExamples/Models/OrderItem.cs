namespace FluentValidationExamples.Models
{
    public class OrderItem
    {
        public string? ProductCode { get; set; }

        public int Quantity { get; set; }

        public decimal UnitPrice { get; set; }
    }
}
