using MinimalEshop.Application.Domain.Entities;
using MinimalEshop.Application.Interface;

namespace MinimalEshop.Application.Service
    {
    public class ProductService
        {
        private readonly IProduct _product;

        public ProductService(IProduct product)
            {
            _product = product;
            }

        public async Task<List<Product>> GetProductAsync()
            {
            return await _product.GetAllAsync();

            }
        public async Task<List<Product>> SearchProductsAsync(string keyword)
            {
            return await _product.SearchAsync(keyword);
            }

        public async Task<Product> CreateProductAsync(Product product)
            {
            return await _product.AddAsync(product);

            }

        public async Task<bool> UpdateProductAsync(Product product)
            {

            return await _product.UpdateAsync(product);
            }

        public async Task<bool> DeleteProductAsync(string ProductId)
            {
            return await _product.DeleteAsync(ProductId);
            }
        }
    }
