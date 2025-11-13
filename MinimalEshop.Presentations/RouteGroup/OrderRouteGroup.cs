using Microsoft.AspNetCore.Mvc;
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
            group.MapPost("/checkout", async (ClaimsPrincipal user, OrderService orderService) =>
            {
                var userId = user.FindFirstValue(ClaimTypes.NameIdentifier);

                var (success, message, data) = await orderService.CheckOutAsync(userId);

                if (!success)
                    return Results.BadRequest(Result.Fail(null, message, StatusCodes.Status400BadRequest));

                return Results.Ok(Result.Ok(data, message, StatusCodes.Status200OK));
            }).RequireAuthorization("UserOrAdmin")
            .WithTags("Order");

            group.MapPost("/paymentprocess", async (ClaimsPrincipal user, [FromBody] PaymentRequest request, OrderService orderService) =>
            {
                var userId = user.FindFirstValue(ClaimTypes.NameIdentifier);

                if (string.IsNullOrEmpty(userId))
                    return Results.Unauthorized();

                var (success, message) = await orderService.ProcessPaymentAsync(userId, request.PaymentProcess);

                if (!success)
                    return Results.BadRequest(Result.Fail(null, message, StatusCodes.Status400BadRequest));

                return Results.Ok(Result.Ok(new { message }, null, StatusCodes.Status200OK));
            })
            .RequireAuthorization("UserOrAdmin")
            .WithTags("Order")
            .WithOpenApi(operation => new(operation)//this .WithOpenApi must be used in the minimalapi but not in the controller
                {
                 Description = "The payment method:<br>" +
                  "1 = UPI,<br>" +
                  "2 = Cash on Delivery,<br>" +
                  "3 = Card,<br>" +
                  "4 = NetBanking"
                 });

            group.MapGet("/details", async (ClaimsPrincipal user, OrderService orderService) =>
            {
                var userId = user.FindFirstValue(ClaimTypes.NameIdentifier);

                if (string.IsNullOrEmpty(userId))
                    return Results.Unauthorized();

                var (success, message, data) = await orderService.GetOrderDetailsAsync(userId);

                if (!success)
                    return Results.BadRequest(Result.Fail(null, message, StatusCodes.Status400BadRequest));

                return Results.Ok(Result.Ok(data, message, StatusCodes.Status200OK));
            })
            .RequireAuthorization("UserOrAdmin")
            .WithTags("Order");

            return group;
            }

        }
    }
