using MinimalEshop.Application.Domain.Enums;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.ComponentModel.DataAnnotations;

namespace MinimalEshop.Application.Domain.Entities
    {
    public class OrderItem
        {
        [Key]
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string OrderItemId { get; set; } = ObjectId.GenerateNewId().ToString();
        public string OrderId { get; set; }
        public string ProductId { get; set; }
        public string Name { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }

        public PaymentMethod PaymentMethod { get; set; }
        public PaymentStatus paymentStatus { get; set; }
        public OrderStatus OrderStatus { get; set; }
        }

    }
