using MinimalEshop.Application.Domain.Entities;
using MinimalEshop.Application.Interface;
using MinimalEshop.Application.Service;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        public async Task AddToCartAsync_ReturnsCartAdded()
        {
            var productId = "prod123";
            var quantity = 2;
            var userId = "user123";

            _cartRepositoryMock.Setup(r => r.AddToCartAsync(productId, quantity, userId))
                         .ReturnsAsync(true);
            var result = await _cartService.AddToCartAsync(productId, quantity, userId);
            Assert.True(result);
        }
        [Fact]
        public async Task GetCartByUserIdAsync_Should_Return_Cart()
        {
            var userId = "user123";
            var cart = new Cart
            {
                ProductId = "prod123",
                Quantity = 1,
                UserId = userId
            };

            _cartRepositoryMock.Setup(r => r.GetCartByUserIdAsync(userId)).ReturnsAsync(cart);

            var result = await _cartService.GetCartByUserIdAsync(userId);
            Assert.NotNull(result);
            Assert.Equal(userId, result.UserId);
            Assert.Equal("prod123", result.ProductId);
            Assert.Equal(1, result.Quantity);

        }

        [Fact]
        public async Task DeleteProductFromCartAsync_Should_Call_DeleteAsync_Once_And_Return_True()
        {
            var productId = "prod123";
            _cartRepositoryMock.Setup(r => r.DeleteAsync(productId)).ReturnsAsync(true);
            var result = await _cartService.DeleteProductFromCartAsync(productId);
            Assert.True(result);
        }
    }
}
