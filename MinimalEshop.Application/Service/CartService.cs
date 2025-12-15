using MinimalEshop.Application.Domain.Entities;
using MinimalEshop.Application.Interface;

namespace MinimalEshop.Application.Service
{
    public class CartService
    {
        private readonly ICart _cart;
        private readonly IProduct _product;
        private readonly ICacheService _cache;

        public CartService(ICart cart, IProduct product, ICacheService cache)
        {
            _cart = cart;
            _product = product;
            _cache = cache;
        }
        public async Task<bool> AddToCartAsync(string idempotency, string productId, int quantity, string userId)
        {
            var cachedResult = await _cache.GetAsync<bool?>(idempotency);
            if (cachedResult != null)
                return cachedResult.Value;

            var product = await _product.GetProductByIdAsync(productId);
            if (product == null)
            {
                await _cache.SetAsync(idempotency, false);
                return false;
            }


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
            var result = await _cart.AddToCartAsync(cart);

            await _cache.SetAsync(idempotency, result);

            await _cache.RemoveAsync($"cart_{userId}");

            return result;
        }

        public async Task<Cart?> GetCartByUserIdAsync(string userId)
        {
            return await _cache.GetOrSetAsync(
                $"cart_{userId}",
                async () =>
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
                });
        }


        public async Task<bool> DeleteProductFromCartAsync(string userId, string productId, int quantity)
        {
            var result = await _cart.DeleteAsync(userId, productId, quantity);
            if (!result)
            {
                return false;
            }
            await _cache.RemoveAsync($"cart_{userId}");
            return true;
        }

    }
}
