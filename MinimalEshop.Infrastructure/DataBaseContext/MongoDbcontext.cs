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
        public IMongoCollection<Category> Categories => _database.GetCollection<Category>("Categories");
        public IMongoCollection<Cart> Carts => _database.GetCollection<Cart>("Carts");
        public IMongoCollection<Order> Orders => _database.GetCollection<Order>("Orders");
        public IMongoCollection<User> Users => _database.GetCollection<User>("Users");
        public IMongoCollection<OrderItem> OrderItems => _database.GetCollection<OrderItem>("OrderItems");
    }
}
