namespace MinimalEshop.Application.DTO
{
    public class CreatePaymentIntentResult
    {
        public string paymentIntentId { get; set; }
        public string clientSecret { get; set; }
    }
}
