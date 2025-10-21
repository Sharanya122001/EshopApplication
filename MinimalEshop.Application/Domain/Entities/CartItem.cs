using MongoDB.Bson.Serialization.Attributes;

public class CartItem
{
    [BsonElement("ProductId")]
    public string ProductId { get; set; }

    [BsonElement("Quantity")]
    public int Quantity { get; set; }
}
