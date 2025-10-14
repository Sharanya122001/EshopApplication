using MinimalEshop.Application.Domain.Enums;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace MinimalEshop.Application.Domain.Entities
{ 
    
    public class Order
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public int OrderId { get; set; }
        public int UserId { get; set; }
        public DateTime OrderDate { get; set; }
        public decimal TotalAmount { get; set; }
        public string Status { get; set; }
        public List<Product> Products { get; set; } = new List<Product>();
        public PaymentMethod PaymentMethod { get; set; }
    }
}
