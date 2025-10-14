using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using MinimalEshop.Application.Domain.Entities;
using MinimalEshop.Application.Interface;
using MinimalEshop.Infrastructure.Context;
using MinimalEshop.Infrastructure.Data;
using MongoDB.Driver;
using MongoDB.Driver.Linq;

namespace MinimalEshop.Infrastructure.Repositories
{
    public class ProductRepository : IProduct
    {
        private readonly IMongoCollection<Product> _products;

        public ProductRepository(MongoDbContext context)
        {
            _products = context.Products;
        }

        public async Task<List<Product>> GetAllAsync()
        {
            return await _products.Find(_ => true).ToListAsync();
        }

        public async Task<Product> AddAsync(Product product)
        {
            await _products.InsertOneAsync(product);
            return product;
        }

        public async Task<bool> UpdateAsync(Product product)
        {
            var result = await _products.ReplaceOneAsync(
                p => p.ProductId == product.ProductId,
                product);

            return result.IsAcknowledged && result.ModifiedCount > 0;
        }

        public async Task<bool> DeleteAsync(string ProductId)
        {
            var result = await _products.DeleteOneAsync(p => p.ProductId == ProductId);
            return result.IsAcknowledged && result.DeletedCount > 0;
        }
    }
}
