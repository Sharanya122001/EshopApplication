using Microsoft.AspNetCore.Mvc;
using MinimalEshop.Application.Domain.Entities;
using MinimalEshop.Application.DTO;
using MinimalEshop.Application.Service;

namespace MinimalEshop.Presentation.RouteGroup
{
    public static class CartRouteGroup
    {
        public static RouteGroupBuilder CartAPI(this RouteGroupBuilder group)
        {
            group.MapPost("/add", async ([FromServices] CartService _service, [FromBody] CartDto cartDto) =>
            {
                var cart = new Cart
                {
                    CartId = cartDto.CartId,
                    ProductId = cartDto.ProductId,
                    Quantity = cartDto.Quantity,
                    UserId = cartDto.UserId
                };

                var created = await _service.AddToCartAsync(cartDto.ProductId, cartDto.Quantity, cartDto.UserId);

                return Results.Ok(created);
            });

            return group;
        }

    }
}
