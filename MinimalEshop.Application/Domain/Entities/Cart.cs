using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace MinimalEshop.Application.Domain.Entities
{
    public class Cart
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string CartId { get; set; }
        public string UserId { get; set; }
        public List<CartItem> Products { get; set; } = new();
    }
}
