using AutoFixture;
using MinimalEshop.Application.Domain.Entities;
using MinimalEshop.Application.Interface;
using MinimalEshop.Application.Service;
using Moq;

namespace MinimalEshop.Application.Test.Services
    {
    public class CartServiceTests
        {
        private readonly CartService _cartService;
        private readonly Mock<IProduct> _productRepositoryMock;
        private readonly Mock<ICart> _cartRepositoryMock;
        private readonly IFixture _fixture;

        public CartServiceTests()
            {
            _fixture = new Fixture();
            _cartRepositoryMock = new Mock<ICart>();
            _productRepositoryMock = new Mock<IProduct>();
            _cartService = new CartService(_cartRepositoryMock.Object, _productRepositoryMock.Object);
            }

        [Fact]
        public async Task AddToCartAsync_Should_CallRepositoryWithCorrectData_AndReturnTrue()
            {
            var productId = _fixture.Create<string>();
            var userId = _fixture.Create<string>();
            var quantity = _fixture.Create<int>();
            var mockProduct = _fixture.Build<Product>()
                                      .With(p => p.ProductId, productId)
                                      .Create();

            _productRepositoryMock
                .Setup(r => r.GetProductByIdAsync(productId))
                .ReturnsAsync(mockProduct);

            _cartRepositoryMock
                .Setup(r => r.AddToCartAsync(It.IsAny<Cart>()))
                .ReturnsAsync(true);

            var result = await _cartService.AddToCartAsync(productId, quantity, userId);

            Assert.True(result);

            _cartRepositoryMock.Verify(r => r.AddToCartAsync(It.Is<Cart>(c =>
                c.UserId == userId &&
                c.Products.Count == 1 &&
                c.Products[0].ProductId == productId &&
                c.Products[0].Quantity == quantity &&
                c.Products[0].Price == mockProduct.Price
            )), Times.Once);
            }

        [Fact]
        public async Task AddToCartAsync_Should_ReturnFalse_WhenRepositoryReturnsFalse()
            {
            var productId = _fixture.Create<string>();
            var userId = _fixture.Create<string>();
            var quantity = _fixture.Create<int>();
            var mockProduct = _fixture.Build<Product>()
                                      .With(p => p.ProductId, productId)
                                      .Create();

            _productRepositoryMock
                .Setup(p => p.GetProductByIdAsync(productId))
                .ReturnsAsync(mockProduct);

            _cartRepositoryMock
                .Setup(r => r.AddToCartAsync(It.IsAny<Cart>()))
                .ReturnsAsync(false);

            var result = await _cartService.AddToCartAsync(productId, quantity, userId);

            Assert.False(result);
            }

        [Fact]
        public async Task GetCartByUserIdAsync_Should_Return_Cart()
            {

            var userId = _fixture.Create<string>();
            var cart = _fixture.Build<Cart>()
                               .With(c => c.UserId, userId)
                               .With(c => c.Products, _fixture.CreateMany<CartItem>(2).ToList())
                               .Create();

            _cartRepositoryMock.Setup(r => r.GetCartByUserIdAsync(userId))
                               .ReturnsAsync(cart);

            var result = await _cartService.GetCartByUserIdAsync(userId);

            Assert.NotNull(result);
            Assert.Equal(userId, result.UserId);
            Assert.Equal(cart.Products.Count, result.Products.Count);
            }

        [Fact]
        public async Task DeleteProductFromCartAsync_ShouldReturnTrue_WhenProductDeleted()
            {

            var userId = _fixture.Create<string>();
            var productId = _fixture.Create<string>();
            var quantity = _fixture.Create<int>();

            _cartRepositoryMock
                .Setup(c => c.DeleteAsync(userId, productId, quantity))
                .ReturnsAsync(true);

            var result = await _cartService.DeleteProductFromCartAsync(userId, productId, quantity);

            Assert.True(result);
            _cartRepositoryMock.Verify(c => c.DeleteAsync(userId, productId, quantity), Times.Once);
            }

        [Fact]
        public async Task DeleteProductFromCartAsync_ShouldReturnFalse_WhenDeleteFails()
            {
            var userId = _fixture.Create<string>();
            var productId = _fixture.Create<string>();
            var quantity = _fixture.Create<int>();

            _cartRepositoryMock
                .Setup(c => c.DeleteAsync(userId, productId, quantity))
                .ReturnsAsync(false);

            var result = await _cartService.DeleteProductFromCartAsync(userId, productId, quantity);

            Assert.False(result);
            _cartRepositoryMock.Verify(c => c.DeleteAsync(userId, productId, quantity), Times.Once);
            }
        }
    }

