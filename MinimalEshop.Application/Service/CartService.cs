using MinimalEshop.Application.Domain.Entities;
using MinimalEshop.Application.Interface;

namespace MinimalEshop.Application.Service
{
    public class CartService
    {
        private readonly ICart _cart;

        public CartService(ICart cart)
        {
            _cart = cart;
        }
        public async Task<bool> AddToCartAsync(string productId, int quantity, string userId)
        {
            var cart = new Cart
            {
                ProductId = productId,
                Quantity = 1,
                UserId = userId
            };

            return await _cart.AddToCartAsync(productId, quantity, userId);
        }
        public async Task<Cart?> GetCartByUserIdAsync(string userId)
        {
            return await _cart.GetCartByUserIdAsync(userId);
        }
        public async Task<bool> DeleteProductFromCartAsync(string ProductId)
        {
            return await _cart.DeleteAsync(ProductId);
        }
    }
}
