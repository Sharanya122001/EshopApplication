using MinimalEshop.Application.Domain.Enums;

namespace MinimalEshop.Application.DTO
    {
    public class PaymentRequest
        {
        public string OrderId { get; set; }
        public string PaymentMethod { get; set; }
        public long Amount { get; set; }
        public string Currency { get; set; }
        public string PaymentMethodId { get; set; }
        }
    }
