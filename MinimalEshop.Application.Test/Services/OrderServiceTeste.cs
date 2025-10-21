using MinimalEshop.Application.Domain.Entities;
using MinimalEshop.Application.Domain.Enums;
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
    public class OrderServiceTeste
    {
        private readonly OrderService _orderService;
        private readonly Mock<IOrder> _orderRepositoryMock;

        public OrderServiceTeste()
        {
            _orderRepositoryMock = new Mock<IOrder>();
            _orderService = new OrderService(_orderRepositoryMock.Object);
        }
        [Fact]
        public async Task CheckOutAsync_ReturnCheckOut()
        {
             var userId = "user123";
            _orderRepositoryMock.Setup(r => r.CheckOutAsync(userId)).ReturnsAsync(true);
            var result = await _orderService.CheckOutAsync(userId);
            Assert.True(result);
        }

        [Fact]
        public async Task ProcessPaymentAsync_ReturnPaymentStatus()
        {
            int orderId = 101;
            var paymentStatus = PaymentStatus.Success;

            _orderRepositoryMock.Setup(r => r.ProcessPaymentAsync(orderId)).ReturnsAsync(paymentStatus);

            var result = await _orderService.ProcessPaymentAsync(orderId);
            Assert.Equal(paymentStatus, result);
        }

        [Fact]
        public async Task CheckOrderDetailsAsync_ReturnCheckOrderDetails()
        {
            int orderId = 101;
            var orderItem = new OrderItem
            {
                OrderId = orderId,
                ProductId = "prod123",
                Quantity = 2
            };

            _orderRepositoryMock.Setup(r => r.CheckOrderDetailsAsync(orderId)).ReturnsAsync(orderItem);

            var result = await _orderService.CheckOrderDetailsAsync(orderId);

            Assert.NotNull(result);
            Assert.Equal(orderId, result.OrderId);
            Assert.Equal("prod123", result.ProductId);
            Assert.Equal(2, result.Quantity);
        }


    }



}
