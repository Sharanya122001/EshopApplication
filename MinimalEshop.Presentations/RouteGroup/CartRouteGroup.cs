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
                    UserId = cartDto.UserId,
                    Products = new List<CartItem>
                    {
                        new CartItem
                        {
                            ProductId = cartDto.ProductId,
                            Quantity = cartDto.Quantity
                        }
                    }
                };

                var created = await _service.AddToCartAsync(
                   cartDto.ProductId,
                   cartDto.Quantity,
                   cartDto.UserId
                );
                return created
                    ? Results.Ok(new { message = "Product added to cart successfully." })
                    : Results.BadRequest(new { message = "Failed to add product to cart." });
            
            }).RequireAuthorization("UserOrAdmin");



            group.MapDelete("/delete", async ([FromServices] CartService _service, [FromQuery] string userId, [FromQuery] string productId) =>
            {
                var deleted = await _service.DeleteProductFromCartAsync(userId, productId);
                return deleted
                    ? Results.Ok(new { message = "Product removed from cart." })
                    : Results.NotFound(new { message = "Product not found in cart." });
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
