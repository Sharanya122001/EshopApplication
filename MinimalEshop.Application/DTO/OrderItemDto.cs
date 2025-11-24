namespace MinimalEshop.Application.DTO
    {
    public class OrderItemDto
        {
        public string OrderItemId { get; set; }
        public string OrderId { get; set; }
        public string ProductId { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        }
    }
