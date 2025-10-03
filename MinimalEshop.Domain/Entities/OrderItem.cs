namespace MinimalEshop.Domain.Entities
{
    public enum PaymentMethod
    {
        Null,
        UPI,
        CashOnDelivery,
        NetBanking,
        Card,
    }
    public enum PaymentStatus
    {
        Pending,
        Success,
        Failed
    }
    public class OrderItem
    {
        public int OrderItemId { get; set; }
        public int OrderId { get; set; }
        public object ProductId { get; set; }
        public object Quantity { get; set; }
        public object Price { get; set; }
        public PaymentMethod PaymentMethod { get; set; }
        public PaymentStatus paymentStatus { get; set; }
        public OrderStatus OrderStatus { get; set; }
    }
    public enum OrderStatus
    {
        Confirmed,
        Failed,
        Pending,
        Shipped,
        Delivered
    }
}
