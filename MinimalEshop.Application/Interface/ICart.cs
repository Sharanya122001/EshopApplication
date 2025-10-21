using MinimalEshop.Application.Domain.Entities;
using MinimalEshop.Application.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinimalEshop.Application.Interface
{
    public interface ICart
    {
        Task<bool> AddToCartAsync(string ProductId, int quantity, string userId);
        Task<bool> DeleteAsync(string productId);
        Task<Cart?> GetCartByUserIdAsync(string userId);
    }
}
