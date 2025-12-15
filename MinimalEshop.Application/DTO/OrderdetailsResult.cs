namespace MinimalEshop.Application.DTO
{
    public class OrderDetailsResult<T>
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public T? Data { get; set; }
    }

}
