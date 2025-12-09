using MinimalEshop.Application.Domain.Entities;
using MinimalEshop.Application.Domain.Enums;

namespace MinimalEshop.Application.Interface
    {
    public interface IOrder
        {
        Task SaveOrderAsync(Order order, List<OrderItem> items);
        Task<List<Cart>> GetUserCartAsync(string userId);
        Task ClearCartAsync(List<Cart> carts);
        Task<Order?> GetLatestOrderAsync(string userId);
        Task UpdateOrderAsync(Order order);
        Task<List<OrderItem>> GetOrderItemsAsync(string orderId);
        }
    }
