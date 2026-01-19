using MinimalEshop.Application.Domain.Entities;
using MinimalEshop.Application.Domain.Enums;
using MinimalEshop.Application.DTO;
using MinimalEshop.Application.Interface;
using MinimalEshop.Presentation.Responses;


namespace MinimalEshop.Application.Service
{
    public class OrderService
    {
        private readonly IOrderRepo _context;
        public OrderService(IOrderRepo context)
        {
            _context = context;
        private readonly SqsOrderPublisher _sqsPublisher;
        private readonly ISnsNotificationService _snsService;
        private readonly string _orderTopicArn;
        private readonly IUserRepo _userRepo;
        private readonly ICounterRepo _counterRepo;
        private readonly IEmailContentBuilder _emailBuilder;
        public OrderService(IOrderRepo context, IUserRepo userRepo, SqsOrderPublisher sqsPublisher, ISnsNotificationService snsService, ICounterRepo counterRepo, IConfiguration configuration, IEmailContentBuilder emailBuilder)
        {
            _context = context;
            _userRepo = userRepo;
            _sqsPublisher = sqsPublisher;
            _snsService = snsService;
            _counterRepo = counterRepo;
            _emailBuilder = emailBuilder;
            _orderTopicArn = configuration["AWS:OrderTopicArn"];
        }

        public async Task<Result<object>> CheckOutAsync(string userId)
        {
            if (string.IsNullOrEmpty(userId))
                return Result<object>.Fail(null, "User not logged in", 401);

            var existingOrder = await _context.GetLatestOrderAsync(userId);

            if (existingOrder != null &&
                existingOrder.PaymentStatus == PaymentStatus.Pending &&
                existingOrder.Status == "Pending")
            {
                return Result<object>.Fail(
                    null,
                    "You already have a pending order. Please complete payment.",
                    409
                );
            }
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

                OrderNumber = order.OrderNumber,
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

            var user = await _userRepo.GetUserByIdAsync(userId);

            var emailMessage = _emailBuilder.BuildOrderPlacedEmail(
        user,
        order,
        orderItems
    );
            await _snsService.PublishMessageAsync(_orderTopicArn, JsonSerializer.Serialize(emailMessage));
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
        public async Task<Result<PaymentRequestResult>> CreatePaymentRequestAsync(string userId, PaymentMethod paymentMethod)
        {
            if (string.IsNullOrWhiteSpace(userId))
                return Result<PaymentRequestResult>.Fail(null, "User not logged in", 401);

            if (!Enum.IsDefined(typeof(PaymentMethod), paymentMethod) ||
                paymentMethod == PaymentMethod.None)
                return Result<PaymentRequestResult>.Fail(null, "Invalid payment method", 400);

            var order = await _context.GetLatestOrderAsync(userId);
            if (order == null)
                return Result<PaymentRequestResult>.Fail(
                    null,
                    "No order found. Please checkout first.",
                    400
                );

            if (order.PaymentStatus == PaymentStatus.Success)
                return Result<PaymentRequestResult>.Fail(
                    null,
                    "Payment already completed.",
                    400
                );

            const decimal StripeMinimumAmount = 0.5m;
            if (order.TotalAmount < StripeMinimumAmount)
                return Result<PaymentRequestResult>.Fail(
                    null,
                    $"Stripe minimum amount not met ({StripeMinimumAmount})",
                    400
                );

            try
            {
                order.PaymentMethod = paymentMethod;
                order.PaymentStatus = PaymentStatus.Pending;
                await _context.UpdateOrderAsync(order);

                await _sqsPublisher.PublishAsync(
                    order.OrderId,
                    userId,
                    order.TotalAmount,
                    paymentMethod.ToString()
                );

                return Result<PaymentRequestResult>.Ok(
                    new PaymentRequestResult
                    {
                        OrderId = order.OrderId
                    },
                    "Payment request created successfully",
                    200
                );
            }
            catch (Exception)
            {
                order.PaymentStatus = PaymentStatus.Failed;
                order.Status = "PaymentInitiationFailed";
                await _context.UpdateOrderAsync(order);

                return Result<PaymentRequestResult>.Fail(
                    null,
                    "Failed to initiate payment. Please try again.",
                    500
                );
            }
        }
        public async Task<Result<object>> UpdatePaymentStatusAsync(string orderId, string paymentStatus, string transactionId)
        {
            if (string.IsNullOrWhiteSpace(orderId))
                return Result<object>.Fail(null, "OrderId is required", 400);

            Order order = null;

            try
            {
                order = await _context.GetOrderByIdAsync(orderId);

                if (order == null)
                    return Result<object>.Fail(null, $"Order not found: {orderId}", 404);

                if (order.PaymentStatus == PaymentStatus.Success ||
                    order.PaymentStatus == PaymentStatus.Failed)
                {
                    return Result<object>.Fail(
                        null,
                        $"Payment already finalized. Status: {order.PaymentStatus}",
                        400
                    );
                }

                if (!Enum.TryParse<PaymentStatus>(paymentStatus, true, out var parsedStatus))
                    return Result<object>.Fail(null, $"Invalid payment status: {paymentStatus}", 400);

                if (parsedStatus != PaymentStatus.Success &&
                    parsedStatus != PaymentStatus.Failed)
                {
                    return Result<object>.Fail(null, "Invalid final payment state", 400);
                }

                order.PaymentStatus = parsedStatus;
                order.TransactionId = transactionId;
                order.Status = parsedStatus == PaymentStatus.Success
                    ? "Completed"
                    : "PaymentFailed";

                await _context.UpdateOrderAsync(order);

                var user = await _userRepo.GetUserByIdAsync(order.UserId);
                var items = await _context.GetOrderItemsAsync(order.OrderId)
                             ?? new List<OrderItem>();

                EmailMessageDto email = parsedStatus == PaymentStatus.Success
                    ? _emailBuilder.BuildPaymentSuccessEmail(user, order, items)
                    : _emailBuilder.BuildPaymentFailedEmail(
                        user,
                        order,
                        items,
                        "Payment was declined by the gateway"
                      );

                await _snsService.PublishMessageAsync(
                    _orderTopicArn,
                    JsonSerializer.Serialize(email)
                );

                return Result<object>.Ok(
                    null,
                    $"Payment {parsedStatus} processed successfully",
                    200
                );
            }
            catch (Exception ex)
            {
                if (order != null)
                {
                    order.PaymentStatus = PaymentStatus.Failed;
                    order.Status = "PaymentProcessingFailed";
                    await _context.UpdateOrderAsync(order);

                    var user = await _userRepo.GetUserByIdAsync(order.UserId);
                    var items = await _context.GetOrderItemsAsync(order.OrderId)
                                 ?? new List<OrderItem>();

                    var failureEmail = _emailBuilder.BuildPaymentFailedEmail(
                        user,
                        order,
                        items,
                        "We encountered a system error while processing your payment."
                    );

                    await _snsService.PublishMessageAsync(
                        _orderTopicArn,
                        JsonSerializer.Serialize(failureEmail)
                    );
                }

                return Result<object>.Fail(
                    null,
                    "Payment processing failed due to a system error",
                    500
                );
            }
        }

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
