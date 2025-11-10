using AutoFixture;
using MinimalEshop.Application.Domain.Enums;
using MinimalEshop.Application.Interface;
using MinimalEshop.Application.Service;
using Moq;

namespace MinimalEshop.Tests.Service
    {
    public class OrderServiceTests
        {
        private readonly Mock<IOrder> _mockOrderRepo;
        private readonly OrderService _orderService;
        private readonly IFixture _fixture;

        public OrderServiceTests()
            {
            _fixture = new Fixture();
            _mockOrderRepo = new Mock<IOrder>();
            _orderService = new OrderService(_mockOrderRepo.Object);
            }

        [Fact]
        public async Task CheckOutAsync_ShouldReturnExpectedResult()
            {
            var userId = _fixture.Create<string>();
            var orderId = _fixture.Create<string>();

            var expectedData = new { OrderId = orderId };
            var expectedResult = (true, "Checkout successful", (object)expectedData);

            _mockOrderRepo
                .Setup(repo => repo.CheckOutAsync(userId))
                .ReturnsAsync(expectedResult);

            var result = await _orderService.CheckOutAsync(userId);

            dynamic data = result.data;

            Assert.True(result.success);
            Assert.Equal("Checkout successful", result.message);
            Assert.Equal(orderId, (string)data.OrderId);

            _mockOrderRepo.Verify(repo => repo.CheckOutAsync(userId), Times.Once);
            }




        [Fact]
        public async Task ProcessPaymentAsync_ShouldReturnExpectedResult()
            {

            var userId = _fixture.Create<string>();
            var paymentMethod = PaymentMethod.UPI;
            var expectedResult = (true, "Payment processed successfully");

            _mockOrderRepo
                .Setup(repo => repo.ProcessPaymentAsync(userId, paymentMethod))
                .ReturnsAsync(expectedResult);

            var result = await _orderService.ProcessPaymentAsync(userId, paymentMethod);

            Assert.True(result.success);
            Assert.Equal("Payment processed successfully", result.message);

            _mockOrderRepo.Verify(repo => repo.ProcessPaymentAsync(userId, paymentMethod), Times.Once);
            }

        [Fact]
        public async Task GetOrderDetailsAsync_Should_Call_Repository_And_Return_Result()
            {
            var userId = _fixture.Create<string>();
            var orderId = _fixture.Create<string>();
            var expectedResult = (true, "Order details fetched successfully", new { OrderId = orderId });

            _mockOrderRepo
                .Setup(r => r.GetOrderDetailsAsync(userId))
                .ReturnsAsync(expectedResult);

            var result = await _orderService.GetOrderDetailsAsync(userId);

            Assert.Equal(expectedResult, result);
            _mockOrderRepo.Verify(r => r.GetOrderDetailsAsync(userId), Times.Once);
            }
        }
    }

