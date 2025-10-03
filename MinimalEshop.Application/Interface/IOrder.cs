using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MinimalEshop.Application.DTO;
using MinimalEshop.Domain.Entities;

namespace MinimalEshop.Application.Interface
{
    public interface IOrder
    {
        Task<bool> CheckOutAsync(int UserId);
        Task<PaymentStatus> ProcessPaymentAsync(int OrderId);
    }
}
