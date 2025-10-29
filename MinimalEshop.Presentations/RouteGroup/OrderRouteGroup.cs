using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using MinimalEshop.Application.Domain.Entities;
using MinimalEshop.Application.Domain.Enums;
using MinimalEshop.Application.DTO;
using MinimalEshop.Application.Service;
using System.Security.Claims;

namespace MinimalEshop.Presentation.RouteGroup
{
    public static class OrderRouteGroup
    {
        public static RouteGroupBuilder OrderAPI(this RouteGroupBuilder group)
        {
            group.MapPost("/checkout", async ( ClaimsPrincipal user,OrderService orderService) =>
            {
                var userId = user.FindFirstValue(ClaimTypes.NameIdentifier);

                var (success, message, data) = await orderService.CheckOutAsync(userId);

                if (!success)
                    return Results.BadRequest(new { message });

                return Results.Ok(new
                {
                    message,
                    data
                });
            }).RequireAuthorization("UserOrAdmin")
            .WithTags("Order");


            group.MapPost("/paymentprocess", async (ClaimsPrincipal user,[FromBody] PaymentRequest request,OrderService orderService) =>
            {
                var userId = user.FindFirstValue(ClaimTypes.NameIdentifier);

                if (string.IsNullOrEmpty(userId))
                    return Results.Unauthorized();

                var (success, message) = await orderService.ProcessPaymentAsync(userId, request.PaymentProcess);

                if (!success)
                    return Results.BadRequest(new { message });

                return Results.Ok(new { message });
            }).RequireAuthorization("UserOrAdmin")
            .WithTags("Order");

            return group;
        }

    }
}
