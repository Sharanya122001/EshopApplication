using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

public class CartItem
{
    [BsonElement("ProductId")]
    public string ProductId { get; set; }

    [BsonElement("Quantity")]
    public int Quantity { get; set; }
    [BsonRepresentation(BsonType.Decimal128)]
    public decimal Price { get; set; }
}
