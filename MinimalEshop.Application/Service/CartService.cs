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
                         Name = product.Name,
                        Quantity = quantity,
                        Price = product.Price 
                    }
                }
                };

            return await _cart.AddToCartAsync(cart);
            }

        public async Task<Cart?> GetCartByUserIdAsync(string userId)
            {
            var cart = await _cart.GetCartByUserIdAsync(userId);
            if (cart == null) return null;

            foreach (var item in cart.Products)
                {
                var product = await _product.GetProductByIdAsync(item.ProductId);
                if (product != null)
                    {
                    item.Name = product.Name;
                    item.Price = product.Price;
                    }
                }

            return cart;
            }


        public async Task<bool> DeleteProductFromCartAsync(string userId, string productId, int quantity)
            {
            return await _cart.DeleteAsync(userId, productId, quantity);
            }

        }
    }
