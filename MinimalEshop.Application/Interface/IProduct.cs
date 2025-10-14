//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using MinimalEshop.Application.DTO;
//using MinimalEshop.Application.Domain.Entities;

//namespace MinimalEshop.Application.Interface
//{
//    public interface IProduct
//    {
//        Task<List<Product>> GetAllAsync();
//        Task<Product> AddAsync(Product product);
//        Task<bool> UpdateAsync(Product product);
//        Task<bool> DeleteAsync(string ProductId);
//    }
//}
using System.Collections.Generic;
using System.Threading.Tasks;
using MinimalEshop.Application.Domain.Entities;

namespace MinimalEshop.Application.Interface
{
    public interface IProduct
    {
        Task<List<Product>> GetAllAsync();
        Task<Product> AddAsync(Product product);
        Task<bool> UpdateAsync(Product product);
        Task<bool> DeleteAsync(string productId);
    }
}
