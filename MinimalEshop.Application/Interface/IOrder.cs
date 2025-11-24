using MinimalEshop.Application.Domain.Entities;
using MinimalEshop.Application.Domain.Enums;

namespace MinimalEshop.Application.Interface
    {
    public interface IOrder
        {
        Task SaveOrderAsync(Order order, List<OrderItem> items);

        //Task<Order?> GetOrderByIdAsync(int orderId);

        Task<List<Cart>> GetUserCartAsync(string userId);
        Task ClearCartAsync(List<Cart> carts);

        //Task<(bool success, string message, object data)> CheckOutAsync(string userId);
        Task<(bool success, string message)> ProcessPaymentAsync(string userId, PaymentMethod paymentMethod);
        Task<(bool success, string message, object data)> GetOrderDetailsAsync(string userId);


        }
    }
