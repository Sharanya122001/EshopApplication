using Microsoft.AspNetCore.Mvc;
using MinimalEshop.Application.Domain.Entities;
using MinimalEshop.Application.DTO;
using MinimalEshop.Application.Service;
using MinimalEshop.Presentation.Responses;
using System.Security.Claims;

namespace MinimalEshop.Presentation.RouteGroup
    {
    public static class OrderRouteGroup
        {
        public static RouteGroupBuilder OrderAPI(this RouteGroupBuilder group)
            {
            group.MapPost("/checkout", async (ClaimsPrincipal user, [FromServices] OrderService orderService) =>
            {
                var userId = user.FindFirstValue(ClaimTypes.NameIdentifier);

                var result = await orderService.CheckOutAsync(userId);

                if (!result.Success)
                    return Results.BadRequest(result);

                return Results.Ok(result);

            }).RequireAuthorization("UserOrAdmin")
            .WithTags("Order");

            group.MapPost("/paymentprocess", async (ClaimsPrincipal user, [FromBody] PaymentRequest request, OrderService orderService, ILoggerFactory loggerFactory) =>
            {
                var logger = loggerFactory.CreateLogger("OrderRouteLogger");
                logger.LogInformation("POST /paymentprocess called");

                var userId = user.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userId))
                    return Results.Unauthorized();

                var (success, message) = await orderService.ProcessPaymentAsync(userId, request.PaymentProcess);

                if (!success)
                    {
                    logger.LogWarning("Payment failed: {Message}", message);
                    return Results.BadRequest(Result.Fail(null, message, StatusCodes.Status400BadRequest));
                    }

                logger.LogInformation("Payment successful with method {Method}", request.PaymentProcess);

                return Results.Ok(Result.Ok(new { message }, null, StatusCodes.Status200OK));

            })
            .RequireAuthorization("UserOrAdmin")
            .WithTags("Order")
            .WithOpenApi(operation => new(operation)
                    {
                     Description =
                     "Payment Method Values:<br>" +
                     "1 = UPI,<br>" +
                     "2 = Cash on Delivery,<br>" +
                     "3 = Card,<br>" +
                     "4 = NetBanking"
                     });

            group.MapGet("/details", async (ClaimsPrincipal user, [FromServices] OrderService orderService, ILoggerFactory loggerFactory) =>
            {
                var logger = loggerFactory.CreateLogger("OrderRouteLogger");
                logger.LogInformation("GET/ Getting Order details");

                var userId = user.FindFirstValue(ClaimTypes.NameIdentifier);

                if (string.IsNullOrEmpty(userId))
                    return Results.Unauthorized();

                var (success, message, data) = await orderService.GetOrderDetailsAsync(userId);

                if (!success)
                    return Results.BadRequest(Result.Fail(null, message, StatusCodes.Status400BadRequest));

                logger.LogInformation("Retrieved Order details");

                return Results.Ok(Result.Ok(data, message, StatusCodes.Status200OK));
            })
            .RequireAuthorization("UserOrAdmin")
            .WithTags("Order");

            return group;
            }
        }
    }
