using Microsoft.Extensions.Caching.Distributed;
using MinimalEshop.Application.Domain.Entities;
using MinimalEshop.Application.Interface;

namespace MinimalEshop.Application.Service
    {
    public class CartService
        {
        private readonly ICart _cart;
        private readonly IProduct _product;
        private readonly IDistributedCache _cache;

        public CartService(ICart cart, IProduct product, IDistributedCache cache)
            {
            _cart = cart;
            _product = product;
            _cache = cache;
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
            var result = await _cart.AddToCartAsync(cart);
            if (result)
                {
                return true;
                }
            await _cache.RemoveAsync($"cart_{userId}");
            await _cache.SetStringAsync($"cart_{userId}", System.Text.Json.JsonSerializer.Serialize(cart), new DistributedCacheEntryOptions
                {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10)
                });

            return true;
            }

        public async Task<Cart?> GetCartByUserIdAsync(string userId)
            {
            var cached = await _cache.GetStringAsync($"cart_{userId}");
            if (cached != null)
                {
                return System.Text.Json.JsonSerializer.Deserialize<Cart>(cached);
                }

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
            await _cache.SetStringAsync($"cart_{userId}", System.Text.Json.JsonSerializer.Serialize(cart), new DistributedCacheEntryOptions
                {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10)
                });

            return cart;
            }


        public async Task<bool> DeleteProductFromCartAsync(string userId, string productId, int quantity)
            {
            var result=await _cart.DeleteAsync(userId, productId, quantity);
            if (!result)
                {
                return false;
                }
                await _cache.RemoveAsync($"cart_{userId}");
            return true;
            }

        }
    }
