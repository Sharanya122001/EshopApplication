namespace MinimalEshop.Application.DTO
    {
    public class OrderDto
        {
        public string OrderId { get; set; }
        public string UserId { get; set; }
        public DateTime OrderDate { get; set; }
        public decimal TotalAmount { get; set; }
        public string Status { get; set; }
        public string PaymentMethod { get; set; }
        public string PaymentStatus { get; set; }
        public List<OrderItemDto> Items { get; set; }
        }
    }
