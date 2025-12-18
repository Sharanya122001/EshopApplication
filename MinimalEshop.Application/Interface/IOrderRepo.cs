using MinimalEshop.Application.Domain.Entities;

namespace MinimalEshop.Application.Interface
{
    public interface IOrderRepo
    {
        Task SaveOrderAsync(Order order, List<OrderItem> items);
        Task<List<Cart>> GetUserCartAsync(string userId);
        Task ClearCartAsync(List<Cart> carts);
        Task<Order?> GetLatestOrderAsync(string userId);
        Task UpdateOrderAsync(Order order);
        Task<List<OrderItem>> GetOrderItemsAsync(string orderId);
    }
}
