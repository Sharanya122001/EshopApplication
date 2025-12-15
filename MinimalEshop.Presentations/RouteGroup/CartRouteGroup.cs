using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using MinimalEshop.Application.Domain.Entities;
using MinimalEshop.Application.DTO;
using MinimalEshop.Application.Service;
using MinimalEshop.Presentation.Responses;
using System.Security.Claims;

namespace MinimalEshop.Presentation.RouteGroup
{
    public static class CartRouteGroup
    {
        public static RouteGroupBuilder CartAPI(this RouteGroupBuilder group)
        {
            group.MapPost("/", async ([FromHeader(Name = "Idempotency-Key")] string idempotencyKey, [FromServices] CartService _service, [FromServices] IValidator<CartDto> validator, [FromBody] CartDto cartDto, HttpContext httpContext, ILoggerFactory loggerFactory) =>
            {
                var logger = loggerFactory.CreateLogger("CartRouteLogger");
                logger.LogInformation("POST/Cart/AddToCart called to add {ProductId} to cart", cartDto.ProductId);

                var validationResult = await validator.ValidateAsync(cartDto);
                if (!validationResult.IsValid)
                {
                    var errors = string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage));
                    return Results.BadRequest(Result.Fail(null, errors, StatusCodes.Status400BadRequest));
                }

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
                    idempotencyKey,
                    cartDto.ProductId,
                    cartDto.Quantity,
                    userId
                );

                logger.LogInformation(created ? "Cart created and Product {ProductId} Added to cart " : "Failed to add to cart", cartDto.ProductId);

                return created
                    ? Results.Ok(Result.Ok(new { message = "Product added to cart successfully." }, null, StatusCodes.Status200OK))
                    : Results.BadRequest(Result.Fail(null, "Failed to add product to cart.", StatusCodes.Status400BadRequest));

            }).RequireAuthorization("UserOrAdmin")
            .WithTags("Cart");

            group.MapDelete("/items/{productId}", async ([FromServices] CartService _service, [FromServices] IValidator<CartDto> validator, [FromQuery] string productId, [FromQuery] int? quantity, HttpContext httpContext, ILoggerFactory loggerFactory) =>
            {
                var logger = loggerFactory.CreateLogger("CartRouteLogger");
                logger.LogInformation("DELETE/Cart/product called to delete {ProductId} from cart", productId);

                var dto = new CartDto
                {
                    ProductId = productId,
                    Quantity = quantity ?? 1
                };

                var validationResult = await validator.ValidateAsync(dto);
                if (!validationResult.IsValid)
                {
                    var errors = string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage));
                    return Results.BadRequest(Result.Fail(null, errors, StatusCodes.Status400BadRequest));
                }

                var userId = httpContext.User.FindFirst("id")?.Value
                             ?? httpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                if (string.IsNullOrEmpty(userId))
                    return Results.Json(Result.Fail(null, "Authentication is required.", StatusCodes.Status401Unauthorized),
                        statusCode: StatusCodes.Status401Unauthorized);

                var qty = quantity ?? 1;

                var deleted = await _service.DeleteProductFromCartAsync(userId, productId, qty);
                logger.LogInformation(deleted ? "Deleted products{productId} from cart" : "Failed to delete {productId} from cart", productId);

                return deleted
                    ? Results.Ok(Result.Ok(new { message = "Product removed from cart successfully." }, null, StatusCodes.Status200OK))
                    : Results.NotFound(Result.Fail(null, "Product not found or quantity invalid.", StatusCodes.Status404NotFound));
            })
             .RequireAuthorization("UserOrAdmin")
             .WithTags("Cart");



            group.MapGet("/", async (HttpContext context, [FromServices] CartService _service, ILoggerFactory loggerFactory) =>
            {
                var logger = loggerFactory.CreateLogger("CartRouteLogger");
                logger.LogInformation("GET/Cart called to get cart")
                ;
                var userId = context.User.FindFirst("id")?.Value
                             ?? context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                if (string.IsNullOrEmpty(userId))
                    return Results.Json(Result.Fail(null, "Authentication is required.", StatusCodes.Status401Unauthorized),
                                        statusCode: StatusCodes.Status401Unauthorized);

                var cart = await _service.GetCartByUserIdAsync(userId);
                if (cart == null)
                    return Results.NotFound(Result.Fail(null, "Cart not found for the specified user.", StatusCodes.Status404NotFound));

                logger.LogInformation("Cart Viewed");
                return Results.Ok(Result.Ok(cart, null, StatusCodes.Status200OK));

            }).RequireAuthorization("UserOrAdmin")
              .WithTags("Cart");

            return group;
        }

    }
}