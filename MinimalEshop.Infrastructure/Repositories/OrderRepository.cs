using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using MinimalEshop.Application.Domain.Entities;
using MinimalEshop.Application.Domain.Enums;
using MinimalEshop.Application.Interface;
using MinimalEshop.Infrastructure.Context;
using MongoDB.Driver;

namespace MinimalEshop.Infrastructure.Repositories
{
    public class OrderRepository : IOrder
    {
        private readonly IMongoCollection<Cart> _cart;
        private readonly IMongoCollection<Order> _order;
        private readonly IMongoCollection<OrderItem> _orderItem;

        public OrderRepository(MongoDbContext context)
        {
            _cart = context.Carts;
            _order = context.Orders;
            _orderItem = context.OrderItems;
        }
        public async Task<(bool success, string message, object data)> CheckOutAsync(string userId)
        {
            var cartList = await _cart.Find(c => c.UserId == userId).ToListAsync();

            if (cartList == null || !cartList.Any())
                return (false, "Your cart is empty. Please add items before checkout.", null);

            var totalAmount = cartList.Sum(c =>
                (c.Products ?? new List<CartItem>())
                .Sum(p => p.Price * p.Quantity)
            );

            var order = new Order
            {
                UserId = userId,
                OrderDate = DateTime.UtcNow,
                TotalAmount = totalAmount,
                Status = "Pending"
            };

            await _order.InsertOneAsync(order);

            var orderItems = new List<OrderItem>();

            foreach (var cart in cartList)
            {
                foreach (var product in cart.Products ?? new List<CartItem>())
                {
                    orderItems.Add(new OrderItem
                    {
                        OrderId = order.OrderId,
                        ProductId = product.ProductId,
                        Quantity = product.Quantity,
                        Price = product.Price
                    });
                }
            }

            if (orderItems.Any())
                await _orderItem.InsertManyAsync(orderItems);

            await _cart.DeleteManyAsync(c => c.UserId == userId);

            var responseData = new
            {
                order.OrderId,
                order.UserId,
                order.OrderDate,
                order.TotalAmount,
                order.Status,
                Items = orderItems.Select(i => new
                {
                    i.ProductId,
                    i.Quantity,
                    i.Price
                })
            };

            return (true, "Checkout successful. Please choose your payment method.", responseData);
        }

        public async Task<(bool success, string message)> ProcessPaymentAsync(string userId, PaymentMethod paymentMethod)
        {
            var order = await _order
                .Find(o => o.UserId == userId)
                .SortByDescending(o => o.OrderDate)
                .FirstOrDefaultAsync();

            if (order == null)
                return (false, "Checkout is pending. Please complete checkout before making payment.");

            var updatePaymentMethod = Builders<Order>.Update
                .Set(o => o.PaymentMethod, paymentMethod);
            await _order.UpdateOneAsync(o => o.OrderId == order.OrderId, updatePaymentMethod);

            PaymentStatus status = paymentMethod switch
            {
                    PaymentMethod.CashOnDelivery => PaymentStatus.Pending,
                    PaymentMethod.UPI or PaymentMethod.NetBanking or PaymentMethod.Card => PaymentStatus.Success,
                    _ => PaymentStatus.Failed
            };

            var updatedStatus = status == PaymentStatus.Success ? "Completed"
                              : status == PaymentStatus.Pending ? "Pending"
                              : "Failed";

            var updateStatus = Builders<Order>.Update
                .Set(o => o.Status, updatedStatus);
            await _order.UpdateOneAsync(o => o.OrderId == order.OrderId, updateStatus);

            var message = $"Payment processed successfully using: {paymentMethod}";
            return (true, message);
        }


    }

}

