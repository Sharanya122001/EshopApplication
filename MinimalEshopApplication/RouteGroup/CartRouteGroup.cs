using MinimalEshop.Application.DTO;
using MinimalEshop.Application.Service;
using MinimalEshop.Domain.Entities;

namespace MinimalEshop.Presentation.RouteGroup
{
    public static class CartRouteGroup
    {
        public static RouteGroupBuilder CartAPI(this RouteGroupBuilder group)
        {
            group.MapPost("/add", async (CartService _service, CartDto cartDto, int ProductId, int Quantity, int UserId) =>
            {
                var cart = new Cart
                {
                    CartId = cartDto.CartId,
                    ProductId = cartDto.ProductId,
                    Quantity = cartDto.Quantity,
                    UserId = cartDto.UserId
                };
                var created = await _service.AddToCartAsync(ProductId, Quantity, UserId);
                return created;

            });
            return group;
        }
    }
}
