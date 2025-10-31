using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using MinimalEshop.Application.Domain.Entities;
using MinimalEshop.Application.DTO;
using MinimalEshop.Application.Service;
using System.Security.Claims;
using MinimalEshop.Presentation.Responses;

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
                    return Results.Json(Result.Fail(null, "Authentication is required.", StatusCodes.Status401Unauthorized), statusCode: StatusCodes.Status401Unauthorized);


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
                    ? Results.Ok(Result.Ok(new { message = "Product added to cart successfully." }, null, StatusCodes.Status200OK))
                    : Results.BadRequest(Result.Fail(null, "Failed to add product to cart.", StatusCodes.Status400BadRequest));

            }).RequireAuthorization("UserOrAdmin")
            .WithTags("Cart");


            group.MapDelete("/delete", async ([FromServices] CartService _service, [FromQuery] string userId, [FromQuery] string productId) =>
            {
                var deleted = await _service.DeleteProductFromCartAsync(userId, productId);
                return deleted
                    ? Results.Ok(Result.Ok(new { message = "Product removed from cart." }, null, StatusCodes.Status200OK))
                    : Results.NotFound(Result.Fail(null, "Product not found in cart.", StatusCodes.Status404NotFound));
            }).RequireAuthorization("UserOrAdmin")
            .WithTags("Cart");


            group.MapGet("/getcart", async (HttpContext context, [FromServices] CartService _service) =>
            {
                var userId = context.User.FindFirst("id")?.Value
                             ?? context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;


                if (string.IsNullOrEmpty(userId))
                {
                    return Results.Json(Result.Fail(null, "Authentication is required.", StatusCodes.Status401Unauthorized), statusCode: StatusCodes.Status401Unauthorized);
                }

                var cart = await _service.GetCartByUserIdAsync(userId);
                if (cart == null)
                {
                    return Results.NotFound(Result.Fail(null, "Cart not found for the specified user.", StatusCodes.Status404NotFound));
                }
                return Results.Ok(Result.Ok(cart, null, StatusCodes.Status200OK));

            }).RequireAuthorization("UserOrAdmin")
            .WithTags("Cart");
            return group;
        }

    }   //standard result needed for each endpoint , generic for get, simple results for other
}
//read about problem details
// public class Result(result<T>){ bool Success;
 //                                  string message
 //                                   List<string> errors
 //                                   int Statuscode
 //                                   T payload