using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using MinimalEshop.Application.Domain.Entities;
using MinimalEshop.Application.DTO;
using MinimalEshop.Application.Service;
using System.Security.Claims;

namespace MinimalEshop.Presentation.RouteGroup
{
    public static class CartRouteGroup
    {
        public static RouteGroupBuilder CartAPI(this RouteGroupBuilder group)
        {
            group.MapPost("/add", async ([FromServices] CartService _service,[FromBody] CartDto cartDto,HttpContext httpContext) =>
            {
                var userId = httpContext.User.FindFirst("id")?.Value
                             ?? httpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                if (string.IsNullOrEmpty(userId))
                    return Results.Unauthorized();


                var cart = new Cart
                {
                   UserId = userId,
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
                    userId
                );

                return created
                    ? Results.Ok(new { message = "Product added to cart successfully." })
                    : Results.BadRequest(new { message = "Failed to add product to cart." });

            }).RequireAuthorization("UserOrAdmin")
            .WithTags("Cart");


            group.MapDelete("/delete", async ([FromServices] CartService _service, [FromQuery] string userId, [FromQuery] string productId) =>
            {
                var deleted = await _service.DeleteProductFromCartAsync(userId, productId);
                return deleted
                    ? Results.Ok(new { message = "Product removed from cart." })
                    : Results.NotFound(new { message = "Product not found in cart." });
            }).RequireAuthorization("UserOrAdmin")
            .WithTags("Cart");


            group.MapGet("/getcart", async (HttpContext context, [FromServices] CartService _service) =>
            {
                var userId = context.User.FindFirst("id")?.Value
                             ?? context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;


                if (string.IsNullOrEmpty(userId))
                {
                    return Results.Unauthorized();
                }

                var cart = await _service.GetCartByUserIdAsync(userId);
                if (cart == null)
                {
                    return Results.NotFound("Cart not found for the specified user.");
                }

                return Results.Ok(cart);

            }).RequireAuthorization("UserOrAdmin")
            .WithTags("Cart");
            return group;
        }

    }   
}
