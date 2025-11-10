namespace MinimalEshop.Application.DTO
    {
    public class ProductDto
        {
        public string ProductId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public DateTime Addedon { get; set; }
        public int CategoryId { get; set; }
        }
    }
