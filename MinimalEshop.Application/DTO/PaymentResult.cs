namespace MinimalEshop.Application.DTO
{
    public class PaymentResult
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public object? Data { get; set; }
    }

}
