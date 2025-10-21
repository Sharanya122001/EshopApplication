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

        public async Task<bool> CheckOutAsync(string UserId)
        {
            var result = await _context.CheckOutAsync(UserId);
            return true;
        }

        public async Task<PaymentStatus> ProcessPaymentAsync(int OrderId)
        {
            return await _context.ProcessPaymentAsync(OrderId);
        }

        public async Task<OrderItem> CheckOrderDetailsAsync(int OrderId)
        {
            return await _context.CheckOrderDetailsAsync(OrderId);

        }


    }
}
