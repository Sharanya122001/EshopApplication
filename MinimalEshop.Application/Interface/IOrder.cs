using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MinimalEshop.Application.DTO;
using MinimalEshop.Application.Domain.Entities;
using MinimalEshop.Application.Domain.Enums;

namespace MinimalEshop.Application.Interface
{
    public interface IOrder
    {
        Task<bool> CheckOutAsync(int UserId);
        Task<PaymentStatus> ProcessPaymentAsync(int OrderId);
        Task<OrderItem> CheckOrderDetailsAsync(int OrderId);
    }
}
