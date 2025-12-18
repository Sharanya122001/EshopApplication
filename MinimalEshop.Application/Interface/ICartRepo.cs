using MinimalEshop.Application.Domain.Entities;

namespace MinimalEshop.Application.Interface
{
    public interface ICartRepo
    {
        Task<bool> AddToCartAsync(Cart cart);
        Task<bool> DeleteAsync(string userId, string productId, int quantity);
        Task<Cart?> GetCartByUserIdAsync(string userId);
    }
}
