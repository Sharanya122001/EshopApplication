using MinimalEshop.Application.Domain.Entities;
using MinimalEshop.Application.Interface;
using MinimalEshop.Application.Service;
using Moq;

namespace MinimalEshop.Application.Test.Services
{
    public class CartServiceTests
    {
        private readonly CartService _cartService;
        private readonly Mock<ICart> _cartRepositoryMock;

        public CartServiceTests()
        {
            _cartRepositoryMock = new Mock<ICart>();
            _cartService = new CartService(_cartRepositoryMock.Object);
        }

        [Fact]
        public async Task AddToCartAsync_ShouldReturnTrue_WhenCartIsAdded()
            {
            var productId = "P123";
            var userId = "U001";
            var quantity = 2;

            _cartRepositoryMock.Setup(c => c.AddToCartAsync(It.IsAny<Cart>())).ReturnsAsync(true);

            var result = await _cartService.AddToCartAsync(productId, quantity, userId);
            Assert.True(result);
            _cartRepositoryMock.Verify(c => c.AddToCartAsync(It.Is<Cart>(
                cart => cart.UserId == userId &&
                        cart.Products.Count == 1 &&
                        cart.Products[0].ProductId == productId &&
                        cart.Products[0].Quantity == quantity
            )), Times.Once);
            }

        [Fact]
        public async Task AddToCartAsync_ShouldReturnFalse_WhenAddFails()
            {

            var productId = "P999";
            var userId = "U002";
            var quantity = 1;

            _cartRepositoryMock.Setup(c => c.AddToCartAsync(It.IsAny<Cart>())).ReturnsAsync(false);

            var result = await _cartService.AddToCartAsync(productId, quantity, userId);

            Assert.False(result);
            }

        [Fact]
        public async Task GetCartByUserIdAsync_Should_Return_Cart()
            {
            var userId = "user123";
            var cart = new Cart
                {
                UserId = userId,
                Products = new List<CartItem>
                {
                    new CartItem { ProductId = "prod123", Quantity = 1 }
                }
                };

            _cartRepositoryMock.Setup(r => r.GetCartByUserIdAsync(userId)).ReturnsAsync(cart);


            var result = await _cartService.GetCartByUserIdAsync(userId);


            Assert.NotNull(result);
            Assert.Equal(userId, result.UserId);
            Assert.Single(result.Products);
            Assert.Equal("prod123", result.Products[0].ProductId);
            Assert.Equal(1, result.Products[0].Quantity);
            }

        [Fact]
        public async Task DeleteProductFromCartAsync_ShouldReturnTrue_WhenProductDeleted()
            {

            var userId = "U001";
            var productId = "P123";
            _cartRepositoryMock.Setup(c => c.DeleteAsync(userId, productId)).ReturnsAsync(true);

            var result = await _cartService.DeleteProductFromCartAsync(userId, productId);

            Assert.True(result);
            _cartRepositoryMock.Verify(c => c.DeleteAsync(userId, productId), Times.Once);
            }

        [Fact]
        public async Task DeleteProductFromCartAsync_ShouldReturnFalse_WhenDeleteFails()
            {

            var userId = "U001";
            var productId = "P999";
            _cartRepositoryMock.Setup(c => c.DeleteAsync(userId, productId)).ReturnsAsync(false);


            var result = await _cartService.DeleteProductFromCartAsync(userId, productId);

            Assert.False(result);
            }
        }
}
