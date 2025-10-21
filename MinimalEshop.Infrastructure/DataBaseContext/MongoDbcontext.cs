using Microsoft.Extensions.Options;
using MinimalEshop.Application.Domain.Entities;
using MinimalEshop.Infrastructure.Data;
using MongoDB.Driver;

namespace MinimalEshop.Infrastructure.Context
{
    public class MongoDbContext
    {
        private readonly IMongoDatabase _database;

        public MongoDbContext(IMongoClient mongoClient, IOptions<MongoDBSettings> settings)
        {
            _database = mongoClient.GetDatabase(settings.Value.DatabaseName);
        }

        public IMongoCollection<Product> Products => _database.GetCollection<Product>("Products");
        public IMongoCollection<Category> Categories => _database.GetCollection<Category>("Categorie");
        public IMongoCollection<Cart> Carts => _database.GetCollection<Cart>("Cart");
        public IMongoCollection<Order> Orders => _database.GetCollection<Order>("Order");
        public IMongoCollection<User> Users => _database.GetCollection<User>("User");
        public IMongoCollection<OrderItem> OrderItems => _database.GetCollection<OrderItem>("OrderItem");
    }
}
