using Microsoft.EntityFrameworkCore;
using MinimalEshop.Application.Domain.Entities;
using MinimalEshop.Application.Interface;
using MinimalEshop.Infrastructure.Context;
using MongoDB.Driver;

namespace MinimalEshop.Infrastructure.Repositories
    {
    public class ProductRepository : IProduct
        {
        private readonly MongoDbContext _context;

        public ProductRepository(MongoDbContext context)
            {
            _context = context;
            }

        public async Task<List<Product>> GetAllAsync()
            {
            return await _context.Products.ToListAsync();
            }

        public async Task<List<Product>> SearchAsync(string keyword)
            {
            if (string.IsNullOrWhiteSpace(keyword))
                return new List<Product>();

            return await _context.Products
                 .Where(p => p.Name.ToLower().Contains(keyword.ToLower()))
                 .ToListAsync();
        }


        public async Task<Product> AddAsync(Product product)
            {
            await _context.AddAsync(product);
            await _context.SaveChangesAsync();
            return product;
            }

        public async Task<bool> UpdateAsync(Product product)
            {
            var existingProduct = await _context.Products
            .FirstOrDefaultAsync(p => p.ProductId == product.ProductId);

            if (existingProduct == null)
                return false;

            _context.Entry(existingProduct).CurrentValues.SetValues(product);
            await _context.SaveChangesAsync();

            return true;
            }

        public async Task<bool> DeleteAsync(string ProductId)
            {
            var product = await _context.Products
            .FirstOrDefaultAsync(p => p.ProductId == ProductId);

            if (product == null)
                return false;

            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
            return true;
            }
        public async Task<Product?> GetProductByIdAsync(string productId)
            {
            return await _context.Products.Where(p => p.ProductId == productId).FirstOrDefaultAsync();
            }
        }
    }
