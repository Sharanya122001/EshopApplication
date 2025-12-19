using AutoFixture;
using MinimalEshop.Application.Domain.Entities;
using MinimalEshop.Application.Domain.Enums;
using MinimalEshop.Application.DTO;
using MinimalEshop.Application.Interface;
using MinimalEshop.Application.Service;
using Moq;

namespace MinimalEshop.Tests.Service
{
    public class OrderServiceTests
    {
        private readonly Mock<IOrderRepo> _mockOrderRepo;
        private readonly OrderService _orderService;
        private readonly IFixture _fixture;

        public OrderServiceTests()
        {
            _fixture = new Fixture();
            _mockOrderRepo = new Mock<IOrderRepo>();
            _orderService = new OrderService(_mockOrderRepo.Object);
        }

        [Fact]
        public async Task CheckOutAsync_ShouldReturnOrderDto_WhenCartHasItems()
        {

            var userId = _fixture.Create<string>();

            var carts = new List<Cart>
            {
                new Cart
                {
                    UserId = userId,
                    Products = new List<CartItem>
                    {
                        new CartItem
                        {
                            ProductId = _fixture.Create<string>(),
                            Name = "Product1",
                            Quantity = 2,
                            Price = 50
                        }
                    }
                }
            };

            _mockOrderRepo
                .Setup(r => r.GetUserCartAsync(userId))
                .ReturnsAsync(carts);

            _mockOrderRepo
                .Setup(r => r.SaveOrderAsync(It.IsAny<Order>(), It.IsAny<List<OrderItem>>()))
                .Returns(Task.CompletedTask);

            _mockOrderRepo
                .Setup(r => r.ClearCartAsync(It.IsAny<List<Cart>>()))
                .Returns(Task.CompletedTask);


            var result = await _orderService.CheckOutAsync(userId);

            Assert.True(result.Success);
            Assert.Equal("Checkout successful", result.Message);
            Assert.NotNull(result.Payload);

            var orderDto = result.Payload as OrderDto;
            Assert.NotNull(orderDto);
            Assert.Equal(userId, orderDto.UserId);
            Assert.Equal(1, orderDto.Items.Count);

            _mockOrderRepo.Verify(r => r.SaveOrderAsync(It.IsAny<Order>(), It.IsAny<List<OrderItem>>()), Times.Once);
            _mockOrderRepo.Verify(r => r.ClearCartAsync(It.IsAny<List<Cart>>()), Times.Once);
        }

        [Fact]
        public async Task ProcessPaymentAsync_ShouldReturnSuccess_WhenPaymentProcessed()
        {

            var userId = _fixture.Create<string>();
            var paymentMethod = PaymentMethod.UPI;

            var order = new Order
            {
                OrderId = _fixture.Create<string>(),
                UserId = userId,
                Name = "Test Order",
                Status = "Pending"
            };

            _mockOrderRepo
                .Setup(r => r.GetLatestOrderAsync(userId))
                .ReturnsAsync(order);

            _mockOrderRepo
                .Setup(r => r.UpdateOrderAsync(order))
                .Returns(Task.CompletedTask);


            var result = await _orderService.ProcessPaymentAsync(userId, paymentMethod);


            Assert.True(result.Success);
            Assert.Contains("Payment processed successfully", result.Message);
            Assert.NotNull(result.Data);

            var updatedOrder = result.Data as Order;
            Assert.Equal(PaymentStatus.Success, updatedOrder.PaymentStatus);
            Assert.Equal("Completed", updatedOrder.Status);
        }

        [Fact]
        public async Task GetOrderDetailsAsync_ShouldReturnOrderDetails_WhenOrderExists()
        {
            var userId = _fixture.Create<string>();

            var order = new Order
            {
                OrderId = _fixture.Create<string>(),
                UserId = userId,
                Name = "Test Order",
                OrderDate = DateTime.UtcNow,
                TotalAmount = 100,
                Status = "Pending",
                PaymentMethod = PaymentMethod.UPI,
                PaymentStatus = PaymentStatus.Pending
            };

            var items = new List<OrderItem>
            {
                new OrderItem
                {
                    ProductId = _fixture.Create<string>(),
                    Name = "Product1",
                    Quantity = 1,
                    Price = 100
                }
            };

            _mockOrderRepo.Setup(r => r.GetLatestOrderAsync(userId)).ReturnsAsync(order);
            _mockOrderRepo.Setup(r => r.GetOrderItemsAsync(order.OrderId)).ReturnsAsync(items);

            var result = await _orderService.GetOrderDetailsAsync(userId);

            Assert.True(result.Success);
            Assert.Equal("Order details fetched successfully.", result.Message);
            Assert.NotNull(result.Data);

            var orderDto = result.Data as OrderDto;
            Assert.NotNull(orderDto);
            Assert.Equal(userId, orderDto.UserId);
            Assert.Equal(1, orderDto.Items.Count);
            Assert.Equal(items[0].ProductId, orderDto.Items[0].ProductId);
        }
    }
}

