//using AutoFixture;
//using MinimalEshop.Application.Domain.Entities;
//using MinimalEshop.Application.Interface;
//using MinimalEshop.Application.Service;
//using Moq;

//namespace MinimalEshop.Application.Test.Services
//    {
//    public class CartServiceTests
//        {
//        private readonly CartService _cartService;
//        private readonly Mock<IProduct> _productRepositoryMock;
//        private readonly Mock<ICart> _cartRepositoryMock;
//        private readonly IFixture _fixture;

//        public CartServiceTests()
//            {
//            _fixture = new Fixture();
//            _cartRepositoryMock = new Mock<ICart>();
//            _productRepositoryMock = new Mock<IProduct>();
//            _cartService = new CartService(_cartRepositoryMock.Object, _productRepositoryMock.Object);
//            }

//        [Fact]
//        public async Task AddToCartAsync_Should_CallRepositoryWithCorrectData_AndReturnTrue()
//            {
//            var productId = _fixture.Create<string>();
//            var userId = _fixture.Create<string>();
//            var quantity = _fixture.Create<int>();
//            var mockProduct = _fixture.Build<Product>()
//                                      .With(p => p.ProductId, productId)
//                                      .Create();

//            _productRepositoryMock
//                .Setup(r => r.GetProductByIdAsync(productId))
//                .ReturnsAsync(mockProduct);

//            _cartRepositoryMock
//                .Setup(r => r.AddToCartAsync(It.IsAny<Cart>()))
//                .ReturnsAsync(true);

//            var result = await _cartService.AddToCartAsync(productId, quantity, userId);

//            Assert.True(result);

//            _cartRepositoryMock.Verify(r => r.AddToCartAsync(It.Is<Cart>(c =>
//                c.UserId == userId &&
//                c.Products.Count == 1 &&
//                c.Products[0].ProductId == productId &&
//                c.Products[0].Quantity == quantity &&
//                c.Products[0].Price == mockProduct.Price
//            )), Times.Once);
//            }

//        [Fact]
//        public async Task AddToCartAsync_Should_ReturnFalse_WhenRepositoryReturnsFalse()
//            {
//            var productId = _fixture.Create<string>();
//            var userId = _fixture.Create<string>();
//            var quantity = _fixture.Create<int>();
//            var mockProduct = _fixture.Build<Product>()
//                                      .With(p => p.ProductId, productId)
//                                      .Create();

//            _productRepositoryMock
//                .Setup(p => p.GetProductByIdAsync(productId))
//                .ReturnsAsync(mockProduct);

//            _cartRepositoryMock
//                .Setup(r => r.AddToCartAsync(It.IsAny<Cart>()))
//                .ReturnsAsync(false);

//            var result = await _cartService.AddToCartAsync(productId, quantity, userId);

//            Assert.False(result);
//            }

//        [Fact]
//        public async Task GetCartByUserIdAsync_Should_Return_Cart()
//            {

//            var userId = _fixture.Create<string>();
//            var cart = _fixture.Build<Cart>()
//                               .With(c => c.UserId, userId)
//                               .With(c => c.Products, _fixture.CreateMany<CartItem>(2).ToList())
//                               .Create();

//            _cartRepositoryMock.Setup(r => r.GetCartByUserIdAsync(userId))
//                               .ReturnsAsync(cart);

//            var result = await _cartService.GetCartByUserIdAsync(userId);

//            Assert.NotNull(result);
//            Assert.Equal(userId, result.UserId);
//            Assert.Equal(cart.Products.Count, result.Products.Count);
//            }

//        [Fact]
//        public async Task DeleteProductFromCartAsync_ShouldReturnTrue_WhenProductDeleted()
//            {

//            var userId = _fixture.Create<string>();
//            var productId = _fixture.Create<string>();
//            var quantity = _fixture.Create<int>();

//            _cartRepositoryMock
//                .Setup(c => c.DeleteAsync(userId, productId, quantity))
//                .ReturnsAsync(true);

//            var result = await _cartService.DeleteProductFromCartAsync(userId, productId, quantity);

//            Assert.True(result);
//            _cartRepositoryMock.Verify(c => c.DeleteAsync(userId, productId, quantity), Times.Once);
//            }

//        [Fact]
//        public async Task DeleteProductFromCartAsync_ShouldReturnFalse_WhenDeleteFails()
//            {
//            var userId = _fixture.Create<string>();
//            var productId = _fixture.Create<string>();
//            var quantity = _fixture.Create<int>();

//            _cartRepositoryMock
//                .Setup(c => c.DeleteAsync(userId, productId, quantity))
//                .ReturnsAsync(false);

//            var result = await _cartService.DeleteProductFromCartAsync(userId, productId, quantity);

//            Assert.False(result);
//            _cartRepositoryMock.Verify(c => c.DeleteAsync(userId, productId, quantity), Times.Once);
//            }
//        }
//    }
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
        private readonly Mock<ICacheService> _cacheMock;
        private readonly IFixture _fixture;

        public CartServiceTests()
            {
            _fixture = new Fixture();

            _fixture.Customize<Product>(c => c
                .With(p => p.ProductId, Guid.NewGuid().ToString())
                .With(p => p.Name, "Test Product")
                .With(p => p.Price, 100)
            );

            _cartRepositoryMock = new Mock<ICart>();
            _productRepositoryMock = new Mock<IProduct>();
            _cacheMock = new Mock<ICacheService>();

            _cartService = new CartService(
                _cartRepositoryMock.Object,
                _productRepositoryMock.Object,
                _cacheMock.Object
            );
            }

        [Fact]
        public async Task AddToCartAsync_Should_CallRepositoryAndReturnTrue()
            {
            var idempotencyKey = "key_123";
            var productId = _fixture.Create<string>();
            var userId = _fixture.Create<string>();
            var quantity = 2;

            var mockProduct = _fixture.Build<Product>()
                                      .With(p => p.ProductId, productId)
                                      .With(p => p.Price, 100)
                                      .Create();

            _cacheMock.Setup(c => c.GetAsync<bool?>(idempotencyKey))
                      .ReturnsAsync((bool?)null);

            _productRepositoryMock
                .Setup(r => r.GetProductByIdAsync(productId))
                .ReturnsAsync(mockProduct);

            _cartRepositoryMock
                .Setup(r => r.AddToCartAsync(It.IsAny<Cart>()))
                .ReturnsAsync(true);

            var result = await _cartService.AddToCartAsync(idempotencyKey, productId, quantity, userId);

            Assert.True(result);

            _cartRepositoryMock.Verify(r => r.AddToCartAsync(It.Is<Cart>(c =>
                c.UserId == userId &&
                c.Products.Count == 1 &&
                c.Products[0].ProductId == productId &&
                c.Products[0].Quantity == quantity &&
                c.Products[0].Price == mockProduct.Price
            )), Times.Once);

            _cacheMock.Verify(c => c.SetAsync(idempotencyKey, true), Times.Once);
            _cacheMock.Verify(c => c.RemoveAsync($"cart_{userId}"), Times.Once);
            }

        [Fact]
        public async Task AddToCartAsync_Should_ReturnFalse_WhenProductNotFound()
            {
            string idempotencyKey = "key_123";
            string productId = "invalid";
            string userId = "user_1";

            _cacheMock.Setup(c => c.GetAsync<bool?>(idempotencyKey))
                      .ReturnsAsync((bool?)null);

            _productRepositoryMock
                .Setup(p => p.GetProductByIdAsync(productId))
                .ReturnsAsync((Product?)null);

            var result = await _cartService.AddToCartAsync(idempotencyKey, productId, 1, userId);

            Assert.False(result);
            _cacheMock.Verify(c => c.SetAsync(idempotencyKey, false), Times.Once);
            }

        [Fact]
        public async Task AddToCartAsync_Should_ReturnCachedValue_WhenIdempotencyAlreadyExists()
            {
            string idempotencyKey = "key_123";

            _cacheMock.Setup(c => c.GetAsync<bool?>(idempotencyKey))
                      .ReturnsAsync(true);

            var result = await _cartService.AddToCartAsync(idempotencyKey, "p1", 1, "u1");

            Assert.True(result);

            _productRepositoryMock.Verify(p => p.GetProductByIdAsync(It.IsAny<string>()), Times.Never);
            _cartRepositoryMock.Verify(c => c.AddToCartAsync(It.IsAny<Cart>()), Times.Never);
            }


        [Fact]
        public async Task GetCartByUserIdAsync_Should_Return_Cart()
            {
            var userId = _fixture.Create<string>();

            var cart = _fixture.Build<Cart>()
                               .With(c => c.UserId, userId)
                               .With(c => c.Products, _fixture.CreateMany<CartItem>(2).ToList())
                               .Create();

            _cacheMock.Setup(c => c.GetOrSetAsync(
                $"cart_{userId}",
                It.IsAny<Func<Task<Cart?>>>()))
                .ReturnsAsync(cart);

            var result = await _cartService.GetCartByUserIdAsync(userId);

            Assert.NotNull(result);
            Assert.Equal(userId, result.UserId);
            }

        [Fact]
        public async Task DeleteProductFromCartAsync_ShouldReturnTrue_WhenDeleted()
            {
            var userId = "user123";
            var productId = "p1";

            _cartRepositoryMock
                .Setup(c => c.DeleteAsync(userId, productId, 1))
                .ReturnsAsync(true);

            var result = await _cartService.DeleteProductFromCartAsync(userId, productId, 1);

            Assert.True(result);
            _cacheMock.Verify(c => c.RemoveAsync($"cart_{userId}"), Times.Once);
            }

        [Fact]
        public async Task DeleteProductFromCartAsync_ShouldReturnFalse_WhenDeleteFails()
            {
            var userId = "user123";
            var productId = "p1";

            _cartRepositoryMock
                .Setup(c => c.DeleteAsync(userId, productId, 1))
                .ReturnsAsync(false);

            var result = await _cartService.DeleteProductFromCartAsync(userId, productId, 1);

            Assert.False(result);
            }
        }
    }

