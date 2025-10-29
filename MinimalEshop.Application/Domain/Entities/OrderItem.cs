using MinimalEshop.Application.Domain.Enums;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace MinimalEshop.Application.Domain.Entities
{
    public class OrderItem
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string OrderItemId { get; set; }
        public string OrderId { get; set; }
        public object ProductId { get; set; }
        public object Quantity { get; set; }
        public object Price { get; set; }

        public PaymentMethod PaymentMethod { get; set; }
        public PaymentStatus paymentStatus { get; set; }
        public OrderStatus OrderStatus { get; set; }
    }
    
}
