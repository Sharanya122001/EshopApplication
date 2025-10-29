using Xunit;
using Moq;
using System.Threading.Tasks;
using MinimalEshop.Application.Service;
using MinimalEshop.Application.Interface;
using MinimalEshop.Application.Domain.Enums;

namespace MinimalEshop.Tests.Service
    {
    public class OrderServiceTests
        {
        private readonly Mock<IOrder> _mockOrderRepo;
        private readonly OrderService _orderService;

        public OrderServiceTests()
            {
            _mockOrderRepo = new Mock<IOrder>();
            _orderService = new OrderService(_mockOrderRepo.Object);
            }

        [Fact]
        public async Task CheckOutAsync_ShouldReturnExpectedResult()
            {
            var userId = "testUser123";
            var expectedResult = (true, "Checkout successful", new { OrderId = "OID123" });

            _mockOrderRepo
                .Setup(repo => repo.CheckOutAsync(userId))
                .ReturnsAsync(expectedResult);

            var result = await _orderService.CheckOutAsync(userId);
            Assert.True(result.success);
            Assert.Equal("Checkout successful", result.message);
            Assert.NotNull(result.data);

            _mockOrderRepo.Verify(repo => repo.CheckOutAsync(userId), Times.Once);
            }

        [Fact]
        public async Task ProcessPaymentAsync_ShouldReturnExpectedResult()
            {
            var userId = "testUser123";
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
            var userId = "user123";
            var expectedResult = (true, "Order details fetched successfully", new { OrderId = "ORD123" });

            _mockOrderRepo
                .Setup(r => r.GetOrderDetailsAsync(userId))
                .ReturnsAsync(expectedResult);

            var result = await _orderService.GetOrderDetailsAsync(userId);

            Assert.Equal(expectedResult, result);
            _mockOrderRepo.Verify(r => r.GetOrderDetailsAsync(userId), Times.Once);
        }
    }
}
