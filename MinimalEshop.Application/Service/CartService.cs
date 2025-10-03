using MinimalEshop.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MinimalEshop.Application.Interface;

namespace MinimalEshop.Application.Service
{
    public class CartService : ICart
    {
        private readonly EshopDbcontext _context;

        public AddToCartService(EshopDbcontext context)
        {
            _context = context;
        }
        public async Task<Cart> AddToCartAsync(int productId, int quantity, int userId)
        {
            var cart = new Cart
            {
                ProductId = productId,
                Quantity = 1,
                UserId = userId
            };

            _context.Carts.Add(cart);
            await _context.SaveChangesAsync();
            return cart;
        }
    }
}
