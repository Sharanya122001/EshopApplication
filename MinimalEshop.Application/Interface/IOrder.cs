using MinimalEshop.Application.Domain.Enums;

namespace MinimalEshop.Application.Interface
    {
    public interface IOrder
        {
        Task<(bool success, string message, object data)> CheckOutAsync(string userId);
        Task<(bool success, string message)> ProcessPaymentAsync(string userId, PaymentMethod paymentMethod);
        Task<(bool success, string message, object data)> GetOrderDetailsAsync(string userId);


        }
    }
