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

        public CartServiceTests()
        {
            _cartRepositoryMock = new Mock<ICart>();
            _productRepositoryMock = new Mock<IProduct>();
            _cartService = new CartService(_cartRepositoryMock.Object, _productRepositoryMock.Object);

            }


        [Fact]
        public async Task AddToCartAsync_Should_CallRepositoryWithCorrectData_AndReturnTrue()
            {
            var productId = "p123";
            var quantity = 2;
            var userId = "u456";

            var mockProduct = new Product
                {
                ProductId = productId,
                Price = 99.99M
                };

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
            var productId = "p1";
            var userId = "u1";
            var quantity = 1;

            var product = new Product
            {
                ProductId = productId,
                Price = 10.0M
            };

            _productRepositoryMock
                .Setup(p => p.GetProductByIdAsync(productId))
                .ReturnsAsync(product);

            _cartRepositoryMock
                .Setup(r => r.AddToCartAsync(It.IsAny<Cart>()))
                .ReturnsAsync(false);

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
