using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MinimalEshop.Application.DTO;
using MinimalEshop.Domain.Entities;

namespace MinimalEshop.Application.Interface
{
    public interface IProduct
    {
        Task<List<Product>> GetProductAsync();
        Task<Product> CreateProductAsync(Product product);
        Task<bool> UpdateProductAsync(int ProductId);
        Task<bool> DeleteProductAsync(int ProductId);
    }
}
