using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace MinimalEshop.Application.Domain.Entities
{
    public class CartItem
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string ProductId { get; set; }
        public int Quantity { get; set; }
    }
}
