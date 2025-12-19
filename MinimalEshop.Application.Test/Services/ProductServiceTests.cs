using AutoFixture;
using MinimalEshop.Application.Domain.Entities;
using MinimalEshop.Application.Interface;
using MinimalEshop.Application.Service;
using Moq;

namespace MinimalEshop.Application.Test.Services
{
    public class ProductServiceTests
    {
        private readonly IFixture _fixture;
        private readonly ProductService _productService;
        private readonly Mock<IProductRepo> _productRepositoryMock;
        private readonly Mock<ICacheService> _cacheMock;

        public ProductServiceTests()
        {
            _fixture = new Fixture();

            _fixture.Customize<Product>(c => c
                .With(p => p.ProductId, Guid.NewGuid().ToString())
                .With(p => p.Name, "Test Product")
                .With(p => p.Price, 100));

            _productRepositoryMock = new Mock<IProductRepo>();
            _cacheMock = new Mock<ICacheService>();

            _productService = new ProductService(
                _productRepositoryMock.Object,
                _cacheMock.Object
            );
        }

        [Fact]
        public async Task GetProductAsync_ReturnsListOfProducts()
        {
            var products = _fixture.CreateMany<Product>(2).ToList();

            _cacheMock.Setup(c => c.GetOrSetAsync(
                "all_products",
                It.IsAny<Func<Task<List<Product>>>>()))
                .ReturnsAsync(products);

            var result = await _productService.GetProductAsync();

            Assert.Equal(2, result.Count);
        }

        [Fact]
        public async Task SearchProductsAsync_ReturnsMatchingProducts()
        {
            var keyword = "Money";
            var products = _fixture.Build<Product>()
                .With(p => p.Name, "Money Plant")
                .CreateMany(2)
                .ToList();

            _cacheMock.Setup(c => c.GetOrSetAsync(
                $"search_{keyword}",
                It.IsAny<Func<Task<List<Product>>>>()))
                .ReturnsAsync(products);

            var result = await _productService.SearchProductsAsync(keyword);

            Assert.Equal(2, result.Count);
        }

        [Fact]
        public async Task SearchProductsAsync_ThrowsException_WhenKeywordEmpty()
        {
            await Assert.ThrowsAsync<ArgumentException>(() =>
                _productService.SearchProductsAsync(""));
        }

        [Fact]
        public async Task CreateProductAsync_ReturnsCreatedProduct()
        {
            var product = _fixture.Create<Product>();

            _cacheMock
                .Setup(c => c.GetOrSetAsync("key123", It.IsAny<Func<Task<Product>>>()))
                .ReturnsAsync(product);

            var result = await _productService.CreateProductAsync("key123", product);

            Assert.Equal(product.ProductId, result.ProductId);
        }

        [Fact]
        public async Task UpdateProductAsync_ReturnsProductUpdated()
        {
            var product = _fixture.Create<Product>();

            _productRepositoryMock
                .Setup(r => r.UpdateAsync(product))
                .ReturnsAsync(true);

            var result = await _productService.UpdateProductAsync(product);

            Assert.True(result);
        }

        [Fact]
        public async Task DeleteProductAsync_ReturnsProductDeleted()
        {
            var productId = Guid.NewGuid().ToString();

            _productRepositoryMock.Setup(r => r.DeleteAsync(productId))
                .ReturnsAsync(true);

            var result = await _productService.DeleteProductAsync(productId);

            Assert.True(result);
        }
    }
}
