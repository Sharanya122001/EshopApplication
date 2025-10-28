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
            var cartItem = new CartItem
            {
                ProductId = productId,
                Quantity = quantity
            };

            var cart = new Cart
            {
                UserId = userId,
                Products = new List<CartItem> { cartItem }
            };

            return await _cart.AddToCartAsync(cart);
        }
        public async Task<Cart?> GetCartByUserIdAsync(string userId)
        {
            return await _cart.GetCartByUserIdAsync(userId);
        }
        public async Task<bool> DeleteProductFromCartAsync(string userId, string productId)
        {
            return await _cart.DeleteAsync(userId, productId);
        }
    }
}
