using Microsoft.EntityFrameworkCore;
using MinimalEshop.Application.Domain.Entities;
using MinimalEshop.Application.Interface;
using MinimalEshop.Infrastructure.Context;
using MongoDB.Driver;

namespace MinimalEshop.Infrastructure.Repositories
    {
    public class CartRepository : ICart
        {
        private readonly MongoDbContext _context;

        public CartRepository(MongoDbContext context)
            {
            _context = context;
            }
        public async Task<bool> AddToCartAsync(Cart cart)
            {
            if (cart == null || string.IsNullOrEmpty(cart.UserId) || cart.Products == null || !cart.Products.Any())
                return false;

            var existingCart = await _context.Carts
                .Include(c => c.Products)
                .FirstOrDefaultAsync(c => c.UserId == cart.UserId);

            if (existingCart == null)
                {
                await _context.Carts.AddAsync(cart);
                }
            else
                {
                var newProduct = cart.Products.First();

                existingCart.Products.Add(newProduct);
                _context.Carts.Update(existingCart);
                }

            await _context.SaveChangesAsync();
            return true;
            }

        public async Task<bool> DeleteAsync(string userId, string productId, int quantity)
            {
            var cart = await _context.Carts
             .Include(c => c.Products)
             .FirstOrDefaultAsync(c => c.UserId == userId);

            if (cart == null) return false;

            var product = cart.Products.FirstOrDefault(p => p.ProductId == productId);
            if (product == null) return false;

            if (quantity >= product.Quantity)
                {
                cart.Products.Remove(product);
                }
            else
                {
                product.Quantity -= quantity;
                }

            _context.Carts.Update(cart);
            await _context.SaveChangesAsync();

            return true;
            }


        public async Task<Cart?> GetCartByUserIdAsync(string userId)
            {
            return await _context.Carts.Where(c => c.UserId.ToString() == userId).FirstOrDefaultAsync();
            }

        }
    }
