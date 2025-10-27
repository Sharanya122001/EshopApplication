using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinimalEshop.Application.DTO
{
    public class AddToCartRequestDto
    {
        public string ProductId { get; set; }
        public int price { get; set; }
        public int Quantity { get; set; }
        public string Name { get; set; }
    }

    public class AddToCartResponseDto : AddToCartRequestDto
    {
        public string Message { get; set; }
    }
}
