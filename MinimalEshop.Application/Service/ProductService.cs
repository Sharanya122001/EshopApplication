using Microsoft.Extensions.Caching.Distributed;
using MinimalEshop.Application.Domain.Entities;
using MinimalEshop.Application.Interface;
using System.Text.Json;

namespace MinimalEshop.Application.Service
    {
    public class ProductService
        {
        private readonly IProduct _product;
        private readonly ICacheService _cache;
        public ProductService(IProduct product, ICacheService cache)
            {
            _product = product;
            _cache = cache;
            }

        public async Task<List<Product>> GetProductAsync()
            {
            return await _cache.GetOrSetAsync("all_products",
            async () => await _product.GetAllAsync());
            }
        public async Task<List<Product>> SearchProductsAsync(string keyword)
            {
            if (string.IsNullOrWhiteSpace(keyword))
                throw new ArgumentException("Keyword cannot be empty");

            return await _cache.GetOrSetAsync($"search_{keyword}",
                async () => await _product.SearchAsync(keyword));
            }

        public async Task<Product> CreateProductAsync(string idempotencyKey, Product product)
            {
            return await _cache.GetOrSetAsync(idempotencyKey,
           async () => await _product.AddAsync(product));

            }

        public async Task<bool> UpdateProductAsync(Product product)
            {

            var result = await _product.UpdateAsync(product);

            if (result)
                await _cache.RemoveAsync("all_products");

            return result;
            }

        public async Task<bool> DeleteProductAsync(string ProductId)
            {
            var result = await _product.DeleteAsync(ProductId);

            if (result)
                await _cache.RemoveAsync("all_products");

            return result;
            }
        }
    }
