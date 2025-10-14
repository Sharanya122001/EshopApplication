using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace MinimalEshop.Application.Domain.Entities
{
    public class User
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string UserId { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public string Role { get; set; } 
    }
}
