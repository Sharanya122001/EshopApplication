using MinimalEshop.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MinimalEshop.Application.Interface;

namespace MinimalEshop.Application.Service
{
    public class ProductService: IProduct
    {
        private readonly EshopDbcontect _context;

        public ProductService(EshopDbcontect context)
        {
            _context = context;
        }

        public async Task<List<Product>> GetProductAsync()
        {
            var query = await _context.Products.AsQueryable();
            return query.ToListAsync();
        }

        public async Task<Product> CreateProductAsync(Product product)
        {
            _context.Product.Add(product);
            await _context.SaveChangesAsync();
            return product;
        }

        public async Task<bool> UpdateProductAsync(int ProductId)
        {
            var product = await _context.Product.FindAsync(ProductId);
            if (product == null)
            {
                return false;
            }
            _context.Product.Update(product);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteProductAsync(int ProductId)
        {
            var product = await _context.Product.FindAsync(ProductId);
            if (product == null)
            {
                return false;
            }
            _context.Product.Remove(product);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
