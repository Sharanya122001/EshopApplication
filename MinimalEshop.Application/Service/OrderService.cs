using MinimalEshop.Application.Domain.Enums;
using MinimalEshop.Application.Interface;

namespace MinimalEshop.Application.Service
    {
    public class OrderService
        {
        private readonly IOrder _context;
        public OrderService(IOrder context)
            {
            _context = context;
            }

        public async Task<(bool success, string message, object data)> CheckOutAsync(string userId)
           => await _context.CheckOutAsync(userId);

        public async Task<(bool success, string message)> ProcessPaymentAsync(string userId, PaymentMethod paymentMethod)
            {
            return await _context.ProcessPaymentAsync(userId, paymentMethod);
            }

        public async Task<(bool success, string message, object data)> GetOrderDetailsAsync(string userId)
    => await _context.GetOrderDetailsAsync(userId);

        }
    }
