using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.ComponentModel.DataAnnotations;

namespace MinimalEshop.Application.Domain.Entities
    {
    public class Cart
        {
        [Key]
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string CartId { get; set; } = ObjectId.GenerateNewId().ToString();
        public string UserId { get; set; }
        public List<CartItem> Products { get; set; } = new();
        }
    }
