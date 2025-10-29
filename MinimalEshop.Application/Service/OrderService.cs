using MinimalEshop.Application.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MinimalEshop.Application.Interface;
using MinimalEshop.Application.Helper;
using MinimalEshop.Application.Domain.Enums;

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

        }
}
