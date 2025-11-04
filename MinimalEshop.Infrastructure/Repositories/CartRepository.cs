using Microsoft.Extensions.Configuration;
using MinimalEshop.Application.Domain.Entities;
using MinimalEshop.Application.Interface;
using MinimalEshop.Application.Service;
using MinimalEshop.Infrastructure.Context;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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


            var existingCart = await _carts.Find(c => c.UserId == cart.UserId).FirstOrDefaultAsync();

            if (existingCart == null)
                {

                await _carts.InsertOneAsync(cart);
                return true;
                }
            var existingProduct = existingCart.Products.FirstOrDefault(p => p.ProductId == cartItem.ProductId);

            if (existingProduct != null)
                {
                var filter = Builders<Cart>.Filter.And(
                    Builders<Cart>.Filter.Eq(c => c.UserId, cart.UserId),
                    Builders<Cart>.Filter.Eq("Products.ProductId", cartItem.ProductId)
                );

                var update = Builders<Cart>.Update
                    .Inc("Products.$.Quantity", cartItem.Quantity);

                await _carts.UpdateOneAsync(filter, update);
                }
            else
                {
                var update = Builders<Cart>.Update.Push(c => c.Products, cartItem);
                await _carts.UpdateOneAsync(c => c.UserId == cart.UserId, update);
                }

            return true;
            }


        public async Task<bool> DeleteAsync(string userId, string productId, int quantity)
            {
            var cart = await _carts.Find(c => c.UserId == userId).FirstOrDefaultAsync();

            if (cart == null || cart.Products == null || !cart.Products.Any())
                return false;

            var product = cart.Products.FirstOrDefault(p => p.ProductId == productId);
            if (product == null)
                return false;

            if (product.Quantity > quantity)
                {
                var filter = Builders<Cart>.Filter.And(
                    Builders<Cart>.Filter.Eq(c => c.UserId, userId),
                    Builders<Cart>.Filter.Eq("Products.ProductId", productId)
                );

                var update = Builders<Cart>.Update.Inc("Products.$.Quantity", -quantity);

                var result = await _carts.UpdateOneAsync(filter, update);
                return result.IsAcknowledged && result.ModifiedCount > 0;
                }
            else
                {
                var update = Builders<Cart>.Update.PullFilter(
                    c => c.Products,
                    p => p.ProductId == productId
                );

                var result = await _carts.UpdateOneAsync(
                    c => c.UserId == userId,
                    update
                );

                return result.IsAcknowledged && result.ModifiedCount > 0;
                }
            }



        public async Task<Cart?> GetCartByUserIdAsync(string userId)
        {
            return await _carts.Find(c => c.UserId.ToString() == userId).FirstOrDefaultAsync();
        }

    }
}
