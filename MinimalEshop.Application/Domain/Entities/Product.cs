using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace MinimalEshop.Application.Domain.Entities
    {
    public class Product
        {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string ProductId { get; set; }
        [BsonElement("Name")]
        public string Name { get; set; } = null!;
        [BsonElement("Description")]
        public string Description { get; set; } = null!;
        [BsonElement("Price")]
        public decimal Price { get; set; }
        [BsonElement("Addedon")]
        public DateTime Addedon { get; set; }
        [BsonElement("CategoryId")]
        public int CategoryId { get; set; }
        }
    }
