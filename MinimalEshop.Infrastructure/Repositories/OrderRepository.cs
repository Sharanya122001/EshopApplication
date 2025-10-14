using Microsoft.Extensions.Configuration;
using MinimalEshop.Application.Domain.Entities;
using MinimalEshop.Application.Domain.Enums;
using MinimalEshop.Application.Interface;
using MinimalEshop.Infrastructure.Context;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        public async Task<bool> CheckOutAsync(int userId)
        {

            var cartList = await _order.Find(c => c.UserId == userId).ToListAsync();

            if (!cartList.Any())
                return false;
            var order = new Order
            {
                UserId = userId,
                OrderDate = DateTime.Now,
                TotalAmount = cartList.Sum(c => c.Products.Sum(p => p.Price))
            };

            await _order.InsertOneAsync(order);

            var orderItems = new List<OrderItem>();
            foreach (var cart in cartList)
            {
                foreach (var product in cart.Products)
                {
                    orderItems.Add(new OrderItem
                    {
                        OrderId = order.OrderId,
                        ProductId = product.ProductId,
                        Quantity = 1,
                        Price = product.Price
                    });
                }
            }

            if (orderItems.Any())
                await _orderItem.InsertManyAsync(orderItems);

            await _cart.DeleteManyAsync(c => c.UserId == userId);

            return true;
        }
        public async Task<PaymentStatus> ProcessPaymentAsync(int orderId)
        { 
            var order = await _order.Find(o => o.OrderId == orderId).FirstOrDefaultAsync();

            if (order == null)
                return PaymentStatus.Failed;

            var paymentMethod = order.PaymentMethod;
            switch (paymentMethod)
            {
                case PaymentMethod.CashOnDelivery:
                    return PaymentStatus.Pending;

                case PaymentMethod.UPI:
                case PaymentMethod.NetBanking:
                case PaymentMethod.Card:
                    return PaymentStatus.Success;

                default:
                    return PaymentStatus.Failed;
            }
        }

        public async Task<OrderItem> CheckOrderDetailsAsync(int orderId)
        {
            var orderItem = await _orderItem
                .Find(oi => oi.OrderId == orderId)
                .FirstOrDefaultAsync();

            return orderItem;
        }

    }

}

