using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using MinimalEshop.Application.Domain.Entities;
using MinimalEshop.Application.DTO;
using MinimalEshop.Application.Service;

namespace MinimalEshop.Presentation.RouteGroup
{
    public static class OrderRouteGroup
    {
        public static RouteGroupBuilder OrderAPI(this RouteGroupBuilder group)
        {
            group.MapPost("/checkout", async ([FromServices] OrderService _service, [FromBody] OrderDto orderDto) =>
            {
                var order = new Order
                    {
                    UserId = orderDto.UserId,
                    //OrderDate = orderDto.OrderDate,
                    TotalAmount = orderDto.TotalAmount,
                    Status = orderDto.Status
                    };

                var checkout = await _service.CheckOutAsync(orderDto.UserId);
                return Results.Ok(checkout);
            }).RequireAuthorization("UserOrAdmin");

            group.MapPost("/payment", async ([FromServices] OrderService _service, [FromBody] OrderDto orderDto) =>
            {
                var order = new Order
                {
                    OrderId = orderDto.OrderId,
                    UserId = orderDto.UserId,
                    //OrderDate = orderDto.OrderDate,
                    TotalAmount = orderDto.TotalAmount,
                    Status = orderDto.Status
                };

                var paymentStatus = await _service.ProcessPaymentAsync(orderDto.OrderId);
                return Results.Ok(paymentStatus);
            }).RequireAuthorization("UserOrAdmin");

            group.MapGet("/details", async ([FromServices] OrderService _service, [FromQuery] string orderId) =>
            {
                //var order = new Order
                //{
                //    OrderId = orderDto.OrderId,
                //    UserId = orderDto.UserId,
                //    OrderDate = orderDto.OrderDate,
                //    TotalAmount = orderDto.TotalAmount,
                //    Status = orderDto.Status
                //};

                var orderDetails = await _service.CheckOrderDetailsAsync(orderId);
                return Results.Ok(orderDetails);
            }).RequireAuthorization("UserOrAdmin");

            return group;
        }

    }
}
