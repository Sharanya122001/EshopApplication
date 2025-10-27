using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
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
            }).RequireAuthorization("UserOrAdmin");


            group.MapDelete("/delete", async ([FromServices] CartService _service, string ProductId) =>
            {
                    var deleted = await _service.DeleteProductFromCartAsync(ProductId);
                    return Results.Ok(deleted);
            }).RequireAuthorization("UserOrAdmin");


            group.MapGet("/getcart", async ([FromServices] CartService _service, [FromQuery] string userId) =>
            {
                var cart = await _service.GetCartByUserIdAsync(userId);
                if (cart == null)
                {
                    return Results.NotFound("Cart not found for the specified user.");
                }
                return Results.Ok(cart);
            }).RequireAuthorization("UserOrAdmin");

            return group;
        }

    }
}
