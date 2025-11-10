using MinimalEshop.Application.Domain.Entities;
using MinimalEshop.Application.Interface;
using MinimalEshop.Infrastructure.Context;
using MongoDB.Driver;

namespace MinimalEshop.Infrastructure.Repositories
    {
    public class CartRepository : ICart
        {
        private readonly IMongoCollection<Cart> _carts;

        public CartRepository(MongoDbContext context)
            {
            _carts = context.Carts;
            }
        public async Task<bool> AddToCartAsync(Cart cart)
            {
            if (cart == null || string.IsNullOrEmpty(cart.UserId) || cart.Products == null || !cart.Products.Any())
                return false;

            var cartItem = cart.Products.First();
            var update = Builders<Cart>.Update.Push(c => c.Products, cartItem);

            await _carts.UpdateOneAsync
            (
                c => c.UserId == cart.UserId,
                update,
                new UpdateOptions { IsUpsert = true }
            );

            return true;
            }

        public async Task<bool> DeleteAsync(string userId, string productId, int quantity)
            {
            var cart = await _carts.Find(c => c.UserId == userId).FirstOrDefaultAsync();
            if (cart == null) return false;

            var product = cart.Products.FirstOrDefault(p => p.ProductId == productId);
            if (product == null) return false;

            if (quantity >= product.Quantity)
                {
                var update = Builders<Cart>.Update.PullFilter(
                    c => c.Products,
                    p => p.ProductId == productId
                );

                await _carts.UpdateOneAsync(c => c.UserId == userId, update);
                }
            else
                {
                var update = Builders<Cart>.Update.Inc("Products.$.Quantity", -quantity);
                await _carts.UpdateOneAsync(
                    c => c.UserId == userId && c.Products.Any(p => p.ProductId == productId),
                    update
                );
                }

            return true;
            }


        public async Task<Cart?> GetCartByUserIdAsync(string userId)
            {
            return await _carts.Find(c => c.UserId.ToString() == userId).FirstOrDefaultAsync();
            }

        }
    }
