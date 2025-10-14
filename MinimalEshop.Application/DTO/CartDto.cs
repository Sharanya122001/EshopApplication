using MinimalEshop.Application.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinimalEshop.Application.DTO
{
    public class CartDto
    {
        public int CartId { get; set; }
        public string ProductId { get; set; }
        public int Quantity { get; set; }
        public int UserId { get; set; }
        //public Product Product { get; set; }
    }
}
