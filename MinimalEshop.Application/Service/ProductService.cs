using Microsoft.Extensions.Caching.Distributed;
using MinimalEshop.Application.Domain.Entities;
using MinimalEshop.Application.Interface;

namespace MinimalEshop.Application.Service
    {
    public class ProductService
        {
        private readonly IProduct _product;
        private readonly IDistributedCache _cache;

        public ProductService(IProduct product, IDistributedCache cache)
            {
            _product = product;
            _cache = cache;
            }

        public async Task<List<Product>> GetProductAsync()
            {
            //trying to read from cache
            var cache=await _cache.GetStringAsync("all_products");

            //returns json value from the list of products
            if (cache!=null)
                {
                return System.Text.Json.JsonSerializer.Deserialize<List<Product>>(cache)!;
                }
            //not in cache get it from db
            var products= await _product.GetAllAsync();
            await _cache.SetStringAsync("all_products", System.Text.Json.JsonSerializer.Serialize(products), new DistributedCacheEntryOptions
                {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10)
                });
            return products;
            }
        public async Task<List<Product>> SearchProductsAsync(string keyword)
            {
            var Cache=await _cache.GetStringAsync($"search_{keyword}");

            if (Cache!=null)
                {
                return System.Text.Json.JsonSerializer.Deserialize<List<Product>>(Cache)!;
                }   
            var products=await _product.SearchAsync(keyword);
            if (string.IsNullOrWhiteSpace(keyword))
                throw new Exception("Keyword cannot be empty");

            await _cache.SetStringAsync($"search_{keyword}", System.Text.Json.JsonSerializer.Serialize(products), new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10)
                });
            return products;
            }

        public async Task<Product> CreateProductAsync(Product product)
            {

            return await _product.AddAsync(product);

            }

        public async Task<bool> UpdateProductAsync(Product product)
            {
            if (product == null || string.IsNullOrEmpty(product.ProductId))
                {
                throw new ArgumentException("Product or ProductId cannot be null");
                }
            var result=await _product.UpdateAsync(product);

            if (!result)
                {
                return false;
                }
            await _cache.RemoveAsync("all_products");
            return true;
            }

        public async Task<bool> DeleteProductAsync(string ProductId)
            {
            var result= await _product.DeleteAsync(ProductId);
            if(!result)
                {
                return false;
                }
            await _cache.RemoveAsync("all_products");
            return true;
            }
        }
    }
