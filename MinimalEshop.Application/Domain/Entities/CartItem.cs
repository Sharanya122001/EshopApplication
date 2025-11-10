using MongoDB.Bson.Serialization.Attributes;

public class CartItem
    {
    [BsonElement("ProductId")]
    public string ProductId { get; set; }
    public string? Name { get; set; }

    [BsonElement("Quantity")]
    public int Quantity { get; set; }
    [BsonElement("Price")]
    public decimal Price { get; set; }

    }
