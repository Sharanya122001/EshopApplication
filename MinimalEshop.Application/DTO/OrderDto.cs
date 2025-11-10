namespace MinimalEshop.Application.DTO
    {
    public class OrderDto
        {
        public string OrderId { get; set; }
        public string UserId { get; set; }
        public decimal TotalAmount { get; set; }
        public string Status { get; set; }
        }
    }
