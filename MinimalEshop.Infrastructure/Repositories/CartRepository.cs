using Microsoft.Extensions.Configuration;
using MinimalEshop.Application.Domain.Entities;
using MinimalEshop.Application.Interface;
using MinimalEshop.Application.Service;
using MinimalEshop.Infrastructure.Context;
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
        public async Task<bool> AddToCartAsync(string ProductId, int quantity, int userId)
        {
            var cartItem = new CartItem
            {
                ProductId = ProductId,
                Quantity = quantity
            };

            await _carts.UpdateOneAsync(
          c => c.UserId == userId,
          Builders<Cart>.Update.Push(c => c.Products, cartItem), 
          new UpdateOptions { IsUpsert = true });
            return true;
          
        }
    }
}
