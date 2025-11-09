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
        Task<bool> AddToCartAsync(Cart cart);
        Task<bool> DeleteAsync(string userId, string productId, int quantity);
        Task<Cart?> GetCartByUserIdAsync(string userId);
    }
}
