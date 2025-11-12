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
        private readonly Mock<IProduct> _productRepositoryMock;
        public ProductServiceTests()
            {
            _fixture = new Fixture();
            _productRepositoryMock = new Mock<IProduct>();
            _productService = new ProductService(_productRepositoryMock.Object);
            }
        [Fact]
        public async Task GetProductAsync_ReturnsListOfProducts()
            {
            var products = _fixture.CreateMany<Product>(2).ToList();
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
            var keyword = "Money";
            var fixture = new Fixture();
            var products = fixture.Build<Product>()
                      .With(p => p.Name, "Money Plant")
                      .With(p => p.Price, 1000)
                      .CreateMany(2)
                      .ToList();

            _productRepositoryMock
                .Setup(repo => repo.SearchAsync(keyword))
                .ReturnsAsync(products);

            var result = await _productService.SearchProductsAsync(keyword);

            Assert.Equal(2, result.Count);
            Assert.All(result, p => Assert.Contains("Money", p.Name));
            _productRepositoryMock.Verify(repo => repo.SearchAsync(keyword), Times.Once);
            }


        [Fact]
        public async Task SearchProductsAsync_ReturnsEmptyList_WhenKeywordIsEmpty()
            {

            var keyword = string.Empty;
            var fixture = new Fixture();

            var emptyProducts = new List<Product>();

            _productRepositoryMock
                .Setup(repo => repo.SearchAsync(keyword))
                .ReturnsAsync(emptyProducts);

            var result = await _productService.SearchProductsAsync(keyword);

            Assert.NotNull(result);
            Assert.Empty(result);
            _productRepositoryMock.Verify(repo => repo.SearchAsync(keyword), Times.Once);
            }


        [Fact]
        public async Task CreateProductAsync_ReturnsCreatedProduct()
            {
            var fixture = new Fixture();
            var product = fixture.Build<Product>()
                     .With(p => p.ProductId, "3")
                     .With(p => p.Name, "Product3")
                     .With(p => p.Price, 50)
                     .Create();

            _productRepositoryMock.Setup(repo => repo.AddAsync(product)).ReturnsAsync(product);
            var result = await _productService.CreateProductAsync(product);

            Assert.Equal(product.ProductId, result.ProductId);
            Assert.Equal(product.Name, result.Name);
            _productRepositoryMock.Verify(repo => repo.AddAsync(product), Times.Once);
            }

        [Fact]
        public async Task UpdateProductAsync_ReturnsProductUpdated()
            {
            var fixture = new Fixture();
            var product = fixture.Build<Product>()
                     .With(p => p.ProductId, "1")
                     .With(p => p.Name, "UpdatedProduct")
                     .With(p => p.Price, 100)
                     .Create();
            _productRepositoryMock.Setup(repo => repo.UpdateAsync(product)).ReturnsAsync(true);
            var result = await _productService.UpdateProductAsync(product);
            Assert.True(result);
            _productRepositoryMock.Verify(repo => repo.UpdateAsync(product), Times.Once);
            }
        [Fact]
        public async Task DeleteProductAsync_ReturnsProductDeleted()
            {
            var fixture = new Fixture();
            var productId = fixture.Create<string>();
            _productRepositoryMock.Setup(repo => repo.DeleteAsync(productId)).ReturnsAsync(true);
            var result = await _productService.DeleteProductAsync(productId);
            Assert.True(result);
            _productRepositoryMock.Verify(repo => repo.DeleteAsync(productId), Times.Once);
            }

        }
    }
