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
        Task<CheckoutResponseDto> CheckOutAsync(string userId);
        Task<(bool success, string message)> ProcessPaymentAsync(string userId, PaymentMethod paymentMethod);
        Task<(bool success, string message, object data)> GetOrderDetailsAsync(string userId);


        }
    }
