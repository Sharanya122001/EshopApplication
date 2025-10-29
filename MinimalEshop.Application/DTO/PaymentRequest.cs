using MinimalEshop.Application.Domain.Entities;
using MinimalEshop.Application.Domain.Enums;

namespace MinimalEshop.Application.DTO
    {
    public class PaymentRequest
        {
        public PaymentMethod PaymentProcess { get; set; }
        }
    }
