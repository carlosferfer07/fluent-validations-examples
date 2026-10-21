namespace FluentValidationExamples.Models
{
    public class Order
    {
        public string? CustomerEmail { get; set; }

        public Address? ShippingAddress { get; set; }

        public List<OrderItem> Items { get; set; } = new();

        public string? DiscountCode { get; set; }

        public decimal Total => Items.Sum(i => i.Quantity * i.UnitPrice);
    }
}
