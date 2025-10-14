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
        public async Task<bool> AddToCartAsync(string productId, int quantity, int userId)
        {
            var cart = new Cart
            {
                ProductId = productId,
                Quantity = 1,
                UserId = userId
            };

            return await _cart.AddToCartAsync(productId, quantity, userId);
        }

        //public Task<AddToBasketResponseDto> AddToCartAsync(AddToCartRequestDto addToCartRequestDto)
        //{
        //    throw new NotImplementedException();
        //}
    }
}
