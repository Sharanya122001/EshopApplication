using MinimalEshop.Application.Domain.Entities;
using MinimalEshop.Application.Domain.Enums;
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

            var carts = await _context.GetUserCartAsync(userId);

            if (carts == null || !carts.Any())
                return Result<object>.Fail(null, "Your cart is empty", 400);

            var totalAmount = carts.Sum(c => c.GetTotalPrice());

            var order = new Order
                {
                UserId = userId,
                Name = "Checkout Order",
                OrderDate = DateTime.UtcNow,
                TotalAmount = totalAmount,
                Status = "Pending"
                };

            var orderItems = carts
                .SelectMany(c => c.Products)
                .Select(p => new OrderItem
                    {
                    ProductId = p.ProductId,
                    Name = string.IsNullOrEmpty(p.Name) ? "Unknown Product" : p.Name,
                    Quantity = p.Quantity,
                    Price = p.Price
                    })
                .ToList();

            await _context.SaveOrderAsync(order, orderItems);

            await _context.ClearCartAsync(carts);

            var response = new
                {
                order.OrderId,
                order.UserId,
                order.OrderDate,
                order.TotalAmount,
                order.Status,
                Items = orderItems.Select(i => new
                    {
                    i.ProductId,
                    i.Name,
                    i.Quantity,
                    i.Price
                    })
                };

            return Result<object>.Ok(response, "Checkout successful", 200);
            }
        public async Task<(bool success, string message)> ProcessPaymentAsync(string userId, PaymentMethod paymentMethod)
            {
            if (paymentMethod < PaymentMethod.UPI || paymentMethod > PaymentMethod.NetBanking)
                return (false, "Invalid payment method. Please choose a valid one.");

            var order = await _context.GetLatestOrderAsync(userId);
            if (order == null)
                return (false, "Checkout is pending. Please complete checkout before making payment.");

            if (order.Status == "Completed")
                return (false, "Payment is already done.");

            order.ProcessPayment(paymentMethod);

            await _context.UpdateOrderAsync(order);

            return (true, $"Payment processed successfully using: {paymentMethod}");
            }

        public async Task<(bool success, string message, object data)> GetOrderDetailsAsync(string userId)
            {
            if (string.IsNullOrWhiteSpace(userId))
                return (false, "Invalid UserId.", null);

            var order = await _context.GetLatestOrderAsync(userId);

            if (order == null)
                return (false, "No orders found for this user.", null);

            var items = await _context.GetOrderItemsAsync(order.OrderId);

            var response = new
                {
                order.OrderId,
                order.UserId,
                order.OrderDate,
                order.TotalAmount,
                order.Status,
                PaymentMethod = order.PaymentMethod.ToString(),
                PaymentStatus = order.PaymentStatus.ToString(),
                Items = items.Select(i => new
                    {
                    i.ProductId,
                    i.Name,
                    i.Quantity,
                    i.Price
                    }).ToList()
                };

            return (true, "Order details fetched successfully.", response);
            }

        }
    }
