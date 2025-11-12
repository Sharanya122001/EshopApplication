using Microsoft.EntityFrameworkCore;
using MinimalEshop.Application.Domain.Entities;
using MinimalEshop.Application.Domain.Enums;
using MinimalEshop.Application.Interface;
using MinimalEshop.Infrastructure.Context;
using MongoDB.Driver;

namespace MinimalEshop.Infrastructure.Repositories
    {
    public class OrderRepository : IOrder
        {
        private readonly MongoDbContext _context;
        public OrderRepository(MongoDbContext context)
            {
            _context = context;
            }
        public async Task<(bool success, string message, object data)> CheckOutAsync(string userId)
            {
            var cartItems = await _context.Carts
                .Where(c => c.UserId == userId)
                .Include(c => c.Products)
                .ToListAsync();

            if (cartItems == null || !cartItems.Any())
                return (false, "Your cart is empty. Please add items before checkout.", null);

            var totalAmount = cartItems.Sum(c =>
                (c.Products ?? new List<CartItem>())
                .Sum(p => p.Price * p.Quantity)
            );

            var order = new Order
                {
                UserId = userId,
                Name = "Checkout Order",
                OrderDate = DateTime.UtcNow,
                TotalAmount = totalAmount,
                Status = "Pending"
                };

            await _context.Orders.AddAsync(order);
            await _context.SaveChangesAsync(); 

            var orderItems = new List<OrderItem>();

            foreach (var cart in cartItems)
                {
                foreach (var product in cart.Products ?? new List<CartItem>())
                    {
                    orderItems.Add(new OrderItem
                        {
                        OrderId = order.OrderId,
                        ProductId = product.ProductId,
                        Name = string.IsNullOrEmpty(product.Name) ? "Unknown Product" : product.Name,
                        Quantity = product.Quantity,
                        Price = product.Price
                        });
                    }
                }

            if (orderItems.Any())
                {
                await _context.OrderItems.AddRangeAsync(orderItems);
                await _context.SaveChangesAsync();
                }

            _context.Carts.RemoveRange(cartItems);
            await _context.SaveChangesAsync();

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
                    i.Name,
                    i.Quantity,
                    i.Price
                    })
                };

            return (true, "Checkout successful. Please choose your payment method.", responseData);
            }
        public async Task<(bool success, string message)> ProcessPaymentAsync(string userId, PaymentMethod paymentMethod)
            {
            if (paymentMethod < PaymentMethod.UPI || paymentMethod > PaymentMethod.Card)
                return (false, "Invalid payment method.");

            var order = await _context.Orders
                .Where(o => o.UserId == userId)
                .OrderByDescending(o => o.OrderDate)
                .FirstOrDefaultAsync();

            if (order == null)
                return (false, "Checkout is pending. Please complete checkout before making payment.");

            if (order.Status == "Completed")
                return (false, "Payment is already done.");

            order.PaymentMethod = paymentMethod;

            if (paymentMethod == PaymentMethod.CashOnDelivery)
                {
                order.PaymentStatus = PaymentStatus.Pending;
                order.Status = "Pending";
                }
            else
                {
                order.PaymentStatus = PaymentStatus.Success;
                order.Status = "Completed";
                }

            _context.Orders.Update(order);
            await _context.SaveChangesAsync();

            var message = $"Payment processed successfully using: {paymentMethod}";
            return (true, message);
            }


        public async Task<(bool success, string message, object data)> GetOrderDetailsAsync(string userId)
            {
            var order = await _context.Orders
                .Where(o => o.UserId == userId)
                .OrderByDescending(o => o.OrderDate)
                .FirstOrDefaultAsync();

            if (order == null)
                return (false, "No orders found for this user.", null);

            var orderItems = await _context.OrderItems
                .Where(oi => oi.OrderId == order.OrderId)
                .ToListAsync();

            var responseData = new
                {
                order.OrderId,
                order.UserId,
                order.OrderDate,
                order.TotalAmount,
                order.Status,
                PaymentMethod = order.PaymentMethod.ToString(),
                PaymentStatus = order.PaymentStatus.ToString(),
                Items = orderItems.Select(i => new
                    {
                    i.ProductId,
                    i.Name,
                    i.Quantity,
                    i.Price
                    }).ToList()
                };

            return (true, "Order details fetched successfully.", responseData);
            }



        }

    }

