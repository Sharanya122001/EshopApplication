using MinimalEshop.Application.Domain.Entities;
using MinimalEshop.Application.Interface;

namespace MinimalEshop.Application.Service
{
    public class CartService
        {
        private readonly ICart _cart;
        private readonly IProduct _product;

        public CartService(ICart cart, IProduct product)
            {
            _cart = cart;
            _product = product;

            }
        public async Task<bool> AddToCartAsync(string productId, int quantity, string userId)
            {
            var product = await _product.GetProductByIdAsync(productId);
            if (product == null)
                return false;

            var cart = new Cart
                {
                UserId = userId,
                Products = new List<CartItem>
        {
            new CartItem
            {
                ProductId = productId,
                Quantity = quantity,
                Price = product.Price
            }
        }
                };

            return await _cart.AddToCartAsync(cart);
            }

        public async Task<Cart?> GetCartByUserIdAsync(string userId)
            {
            return await _cart.GetCartByUserIdAsync(userId);
            }
        public async Task<bool> DeleteAsync(string userId, string productId, int quantity)
            {
            return await _cart.DeleteAsync(userId, productId, quantity);
            }
        }
        }
