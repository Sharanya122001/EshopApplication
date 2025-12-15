using MinimalEshop.Application.Domain.Entities;
using MinimalEshop.Application.Domain.Enums;
using MinimalEshop.Application.DTO;
using MinimalEshop.Application.Interface;
using MinimalEshop.Presentation.Responses;


namespace MinimalEshop.Application.Service
{
    public class OrderService
    {
        private readonly IOrder _context;
        public OrderService(IOrder context)
        {
            _context = context;
        }

        public async Task<Result<object>> CheckOutAsync(string userId)
        {
            if (string.IsNullOrEmpty(userId))
                return Result<object>.Fail(null, "User not logged in", 401);

            // Always get the user's cart from the database
            var carts = await _context.GetUserCartAsync(userId);

            if (carts == null || !carts.Any())
                return Result<object>.Fail(null, "Your cart is empty", 400);

            // Assuming you store multiple carts or a list of carts, merge all products
            var allProducts = carts
                .Where(c => c.Products != null)
                .SelectMany(c => c.Products)
                .ToList();

            if (!allProducts.Any())
                return Result<object>.Fail(null, "Your cart is empty", 400);

            var totalAmount = allProducts.Sum(p => p.Price * p.Quantity);

            var order = new Order
            {
                UserId = userId,
                Name = "Checkout Order",
                OrderDate = DateTime.UtcNow,
                TotalAmount = totalAmount,
                Status = "Pending"
            };

            var orderItems = allProducts.Select(p => new OrderItem
            {
                ProductId = p.ProductId,
                Name = string.IsNullOrEmpty(p.Name) ? "Unknown Product" : p.Name,
                Quantity = p.Quantity,
                Price = p.Price
            }).ToList();

            // Save the order
            await _context.SaveOrderAsync(order, orderItems);

            // Clear the cart only from the database (no Redis now)
            await _context.ClearCartAsync(carts);

            var orderDto = new OrderDto
            {
                OrderId = order.OrderId,
                UserId = order.UserId,
                OrderDate = order.OrderDate,
                TotalAmount = order.TotalAmount,
                Status = order.Status,
                Items = orderItems.Select(i => new OrderItemDto
                {
                    ProductId = i.ProductId,
                    Name = i.Name,
                    Quantity = i.Quantity,
                    Price = i.Price
                }).ToList()
            };

            return Result<object>.Ok(orderDto, "Checkout successful", 200);
        }

        public async Task<PaymentResult> ProcessPaymentAsync(string userId, PaymentMethod paymentMethod)
        {
            if (!Enum.IsDefined(typeof(PaymentMethod), paymentMethod) || paymentMethod == PaymentMethod.None)
            {
                return new PaymentResult
                {
                    Success = false,
                    Message = "Invalid payment method."
                };
            }


            var order = await _context.GetLatestOrderAsync(userId);

            if (order == null)
                return new PaymentResult
                {
                    Success = false,
                    Message = "Checkout is pending. Please complete checkout before making payment."
                };

            if (order.Status == "Completed")
                return new PaymentResult
                {
                    Success = false,
                    Message = "Payment is already done."
                };

            order.ProcessPayment(paymentMethod);
            await _context.UpdateOrderAsync(order);

            return new PaymentResult
            {
                Success = true,
                Message = $"Payment processed successfully using: {paymentMethod}",
                Data = order
            };
        }


        public async Task<Order?> GetLatestOrderAsync(string userId)
        {
            var result = await _context.GetLatestOrderAsync(userId);
            return result;
        }

        public async Task<OrderDetailsResult<OrderDto>> GetOrderDetailsAsync(string userId)
        {
            if (string.IsNullOrWhiteSpace(userId))
                return new OrderDetailsResult<OrderDto>
                {
                    Success = false,
                    Message = "Invalid UserId."
                };

            var order = await _context.GetLatestOrderAsync(userId);

            if (order == null)
                return new OrderDetailsResult<OrderDto>
                {
                    Success = false,
                    Message = "No orders found for this user."
                };

            var items = await _context.GetOrderItemsAsync(order.OrderId);

            var orderDto = new OrderDto
            {
                OrderId = order.OrderId,
                UserId = order.UserId,
                OrderDate = order.OrderDate,
                TotalAmount = order.TotalAmount,
                Status = order.Status,
                PaymentMethod = order.PaymentMethod.ToString(),
                PaymentStatus = order.PaymentStatus.ToString(),
                Items = items.Select(i => new OrderItemDto
                {
                    ProductId = i.ProductId,
                    Name = i.Name,
                    Quantity = i.Quantity,
                    Price = i.Price
                }).ToList()
            };

            return new OrderDetailsResult<OrderDto>
            {
                Success = true,
                Message = "Order details fetched successfully.",
                Data = orderDto
            };
        }


    }
}
