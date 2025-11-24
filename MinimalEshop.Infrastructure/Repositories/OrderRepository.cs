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
        public async Task<List<Cart>> GetUserCartAsync(string userId)
            {
            return await _context.Carts
                .Include(c => c.Products)
                .Where(c => c.UserId == userId)
                .ToListAsync();
            }

        public async Task SaveOrderAsync(Order order, List<OrderItem> items)
            {
            await _context.Orders.AddAsync(order);
            await _context.OrderItems.AddRangeAsync(items);
            await _context.SaveChangesAsync();
            }

        public async Task ClearCartAsync(List<Cart> carts)
            {
            _context.Carts.RemoveRange(carts);
            await _context.SaveChangesAsync();
            }

        public async Task<(bool success, string message)> ProcessPaymentAsync(string userId, PaymentMethod paymentMethod)
            {
            if (paymentMethod < PaymentMethod.UPI)
                return (false, "None.Please choose the valid PaymentMethod.");
            if (paymentMethod > PaymentMethod.Card)
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

