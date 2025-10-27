using MinimalEshop.Application.Domain.Entities;
using MinimalEshop.Application.Interface;
using MinimalEshop.Application.Service;
using Moq;

namespace MinimalEshop.Application.Test.Services
{
    public class ProductServiceTests
    {
        private readonly ProductService _productService;
        private readonly Mock<IProduct> _productRepositoryMock;

        public ProductServiceTests()
        {
            _productRepositoryMock = new Mock<IProduct>();
            _productService = new ProductService(_productRepositoryMock.Object);
        }
        [Fact]
        public async Task GetProductAsync_ReturnsListOfProducts()
        {
            var products = new List<Product>
            {
                new Product { ProductId = "1", Name = "Product1" },
                new Product { ProductId = "2", Name = "Product2" }
            };
            _productRepositoryMock.Setup(repo => repo.GetAllAsync())
                .ReturnsAsync(products);

            var result = await _productService.GetProductAsync(); 
            Assert.Equal(2, result.Count);
            Assert.Equal(products[0].ProductId, result[0].ProductId);
            Assert.Equal(products[1].Name, result[1].Name);

        }
       
        [Fact]
        public async Task SearchProductsAsync_ReturnsMatchingProducts()
        {
            var keyword = "Laptop";
            var products = new List<Product>
            {
                 new Product { ProductId = "1", Name = "Gaming Laptop", Price = 1000 },
                 new Product { ProductId = "2", Name = "Business Laptop", Price = 900 }
            };

            _productRepositoryMock
                .Setup(repo => repo.SearchAsync(keyword))
                .ReturnsAsync(products);

            var result = await _productService.SearchProductsAsync(keyword);

            Assert.Equal(2, result.Count);
            Assert.Contains(result, p => p.Name.Contains("Laptop"));
            _productRepositoryMock.Verify(repo => repo.SearchAsync(keyword), Times.Once);
        }

        [Fact]
        public async Task SearchProductsAsync_ReturnsEmptyList_WhenKeywordIsEmpty()
        {
            var keyword = "";
            _productRepositoryMock
                .Setup(repo => repo.SearchAsync(keyword))
                .ReturnsAsync(new List<Product>());

            var result = await _productService.SearchProductsAsync(keyword);

            Assert.Empty(result);
            _productRepositoryMock.Verify(repo => repo.SearchAsync(keyword), Times.Once);
        }


        [Fact]
        public async Task CreateProductAsync_ReturnsCreatedProduct()
        {
            var product = new Product { ProductId = "3", Name = "Product3", Price = 50 };
            _productRepositoryMock.Setup(repo=>repo.AddAsync(product)).ReturnsAsync(product);
            var result = await _productService.CreateProductAsync(product);

            Assert.Equal(product.ProductId, result.ProductId);
            Assert.Equal(product.Name, result.Name);
            _productRepositoryMock.Verify(repo => repo.AddAsync(product), Times.Once);
        }

        [Fact]
        public async Task UpdateProductAsync_ReturnsProductUpdated()
        {
            var product = new Product { ProductId = "1", Name = "UpdatedProduct", Price = 100 };
            _productRepositoryMock.Setup(repo => repo.UpdateAsync(product)).ReturnsAsync(true);
            var result = await _productService.UpdateProductAsync(product);
            Assert.True(result);
            _productRepositoryMock.Verify(repo => repo.UpdateAsync(product), Times.Once);
        }
        [Fact]
        public async Task DeleteProductAsync_ReturnsProductDeleted()
        {
            var productId = "1";
            _productRepositoryMock.Setup(repo => repo.DeleteAsync(productId)).ReturnsAsync(true);
            var result = await _productService.DeleteProductAsync(productId);
            Assert.True(result);
            _productRepositoryMock.Verify(repo => repo.DeleteAsync(productId), Times.Once);
        }

    }
}
