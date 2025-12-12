using Microsoft.EntityFrameworkCore;
using MinimalEshop.Application.Domain.Entities;
using MinimalEshop.Application.Domain.Enums;
using MinimalEshop.Application.Interface;
using MinimalEshop.Infrastructure.Context;
using MongoDB.Bson;
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
            await _context.SaveChangesAsync();

            foreach (var item in items)
                {
                item.OrderId = order.OrderId;
                item.OrderItemId = ObjectId.GenerateNewId().ToString();
                }

            await _context.OrderItems.AddRangeAsync(items);
            await _context.SaveChangesAsync();
            }

        public async Task ClearCartAsync(List<Cart> carts)
            {
            _context.Carts.RemoveRange(carts);
            await _context.SaveChangesAsync();
            }
        public async Task<Order?> GetLatestOrderAsync(string userId)
            {
            return await _context.Orders
                .Where(o => o.UserId == userId)
                .OrderByDescending(o => o.OrderDate)
                .FirstOrDefaultAsync();
            }

        public async Task UpdateOrderAsync(Order order)
            {
            _context.Orders.Update(order);
            await _context.SaveChangesAsync();
            }
        public async Task<List<OrderItem>> GetOrderItemsAsync(string orderId)
            {
            return await _context.OrderItems
                .Where(oi => oi.OrderId == orderId)
                .ToListAsync();
            }

        }

    }

