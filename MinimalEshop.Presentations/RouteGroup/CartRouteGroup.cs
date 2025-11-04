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
            group.MapPost("/add", async ([FromServices] CartService _service, [FromBody] CartDto cartDto, HttpContext httpContext) =>
            {
                var userId = httpContext.User.FindFirst("id")?.Value
                             ?? httpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                if (string.IsNullOrEmpty(userId))
                    return Results.Unauthorized();

                var created = await _service.AddToCartAsync(
                    cartDto.ProductId,
                    cartDto.Quantity,
                    userId
                );

                return created
                    ? Results.Ok(new { message = "Product added or updated in cart successfully." })
                    : Results.BadRequest(new { message = "Failed to add product to cart." });

            }).RequireAuthorization("UserOrAdmin")
              .WithTags("Cart");



            group.MapDelete("/delete/{productId}/{quantity}", async (
    [FromServices] CartService _service,
    HttpContext httpContext,
    string productId,
    int quantity
) =>
            {
                var userId = httpContext.User.FindFirst("id")?.Value
                             ?? httpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                if (string.IsNullOrEmpty(userId))
                    return Results.Unauthorized();

                var deleted = await _service.DeleteAsync(userId, productId, quantity);

                return deleted
                    ? Results.Ok(new { message = $"Removed {quantity} quantity of product {productId} from cart." })
                    : Results.BadRequest(new { message = "Failed to remove product from cart." });
            })
.RequireAuthorization("UserOrAdmin")
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
