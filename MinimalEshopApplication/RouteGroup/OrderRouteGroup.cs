using MinimalEshop.Application.DTO;
using MinimalEshop.Application.Service;
using MinimalEshop.Domain.Entities;

namespace MinimalEshop.Presentation.RouteGroup
{
    public static class OrderRouteGroup
    {
        public static RouteGroupBuilder OrderAPI(this RouteGroupBuilder group)
        {
            group.MapPost("/checkout/{UserId}", async (OrderService _service, int UserId, OrderDto orderDto) =>
            {
                var order = new Order
                {
                    UserId = orderDto.UserId,
                    OrderDate = orderDto.OrderDate,
                    TotalAmount = orderDto.TotalAmount,
                    Status = orderDto.Status
                };
                var checkout = await _service.CheckOutAsync(UserId);
                return checkout;
            });

            group.MapPost("/{OrderId}/payment", async (OrderService _service, int OrderId, OrderDto orderDto) =>
            {
                var order = new Order
                {
                    OrderId = orderDto.OrderId,
                    UserId = orderDto.UserId,
                    OrderDate = orderDto.OrderDate,
                    TotalAmount = orderDto.TotalAmount,
                    Status = orderDto.Status
                };
                var paymentStatus = await _service.ProcessPaymentAsync(OrderId);
                return paymentStatus;
            });

            group.MapGet("/{OrderId}", async (OrderService _service, int OrderId, OrderDto orderDto) =>
            {
                var order = new Order
                {
                    OrderId = orderDto.OrderId,
                    UserId = orderDto.UserId,
                    OrderDate = orderDto.OrderDate,
                    TotalAmount = orderDto.TotalAmount,
                    Status = orderDto.Status
                };
                var orderDetails = await _service.CheckOrderDetailsAsync(OrderId);
                return orderDetails;    
            });

            return group;
        }
    }
}
