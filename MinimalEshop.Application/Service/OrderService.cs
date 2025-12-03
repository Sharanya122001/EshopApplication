using Microsoft.Extensions.Caching.Distributed;
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
        //public async Task<Result<object>> CheckOutAsync(string userId)
        //    {
        //    if (string.IsNullOrEmpty(userId))
        //        return Result<object>.Fail(null, "User not logged in", 401);


        //    var cache=await _cache.GetStringAsync($"cart_{userId}");
        //    List<Cart>? carts;
        //    if (cache!=null)
        //        {
        //        carts= System.Text.Json.JsonSerializer.Deserialize<List<Cart>>(cache);
        //        }
        //    else
        //        {
        //        carts = await _context.GetUserCartAsync(userId);

        //        if (carts != null && carts.Any())
        //            {
        //            await _cache.SetStringAsync($"cart_{userId}", System.Text.Json.JsonSerializer.Serialize(carts), new DistributedCacheEntryOptions
        //                {
        //                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10)
        //                });
        //            }
        //        }

        //    if (carts == null || !carts.Any())
        //        return Result<object>.Fail(null, "Your cart is empty", 400);

        //    var totalAmount = carts.Sum(c => c.GetTotalPrice());

        //    var order = new Order
        //        {
        //        UserId = userId,
        //        Name = "Checkout Order",
        //        OrderDate = DateTime.UtcNow,
        //        TotalAmount = totalAmount,
        //        Status = "Pending"
        //        };

        //    var orderItems = carts
        //        .Where(c => c.Products != null)
        //        .SelectMany(c => c.Products)
        //        .Select(p => new OrderItem
        //            {
        //            ProductId = p.ProductId,
        //            Name = string.IsNullOrEmpty(p.Name) ? "Unknown Product" : p.Name,
        //            Quantity = p.Quantity,
        //            Price = p.Price
        //            })
        //        .ToList();

        //    await _context.SaveOrderAsync(order, orderItems);

        //    await _context.ClearCartAsync(carts);
        //    await _cache.RemoveAsync($"cart_{userId}");

        //    var orderDto = new OrderDto
        //        {
        //        OrderId = order.OrderId,
        //        UserId = order.UserId,
        //        OrderDate = order.OrderDate,
        //        TotalAmount = order.TotalAmount,
        //        Status = order.Status,
        //        Items = orderItems.Select(i => new OrderItemDto
        //            {
        //            ProductId = i.ProductId,
        //            Name = i.Name,
        //            Quantity = i.Quantity,
        //            Price = i.Price
        //            }).ToList()
        //        };

        //    return Result<object>.Ok(orderDto, "Checkout successful", 200);
        //    }

        //public async Task<Result<object>> CheckOutAsync(string userId)
        //    {
        //    if (string.IsNullOrEmpty(userId))
        //        return Result<object>.Fail(null, "User not logged in", 401);

        //    var cacheKey = $"cart_{userId}";
        //    List<Cart>? carts = null;

        //    // Try to get the cart from Redis cache
        //    var cache = await _cache.GetStringAsync(cacheKey);
        //    if (!string.IsNullOrWhiteSpace(cache))
        //        {
        //        carts = System.Text.Json.JsonSerializer.Deserialize<List<Cart>>(cache,
        //            new System.Text.Json.JsonSerializerOptions
        //                {
        //                PropertyNameCaseInsensitive = true
        //                });
        //        }
        //    else
        //        {
        //        carts = await _context.GetUserCartAsync(userId);

        //        if (carts != null && carts.Any())
        //            {
        //            await _cache.SetStringAsync(cacheKey,
        //                System.Text.Json.JsonSerializer.Serialize(carts),
        //                new DistributedCacheEntryOptions
        //                    {
        //                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10)
        //                    });
        //            }
        //        }

        //    if (carts == null || !carts.Any())
        //        return Result<object>.Fail(null, "Your cart is empty", 400);

        //    var totalAmount = carts.Sum(c => c.GetTotalPrice());

        //    var order = new Order
        //        {
        //        UserId = userId,
        //        Name = "Checkout Order",
        //        OrderDate = DateTime.UtcNow,
        //        TotalAmount = totalAmount,
        //        Status = "Pending"
        //        };

        //    // Minor change: avoid null Products
        //    var orderItems = carts
        //        .Where(c => c.Products != null)
        //        .SelectMany(c => c.Products)
        //        .Select(p => new OrderItem
        //            {
        //            ProductId = p.ProductId,
        //            Name = string.IsNullOrEmpty(p.Name) ? "Unknown Product" : p.Name,
        //            Quantity = p.Quantity,
        //            Price = p.Price
        //            })
        //        .ToList();

        //    await _context.SaveOrderAsync(order, orderItems);

        //    await _context.ClearCartAsync(carts);
        //    await _cache.RemoveAsync(cacheKey);

        //    var orderDto = new OrderDto
        //        {
        //        OrderId = order.OrderId,
        //        UserId = order.UserId,
        //        OrderDate = order.OrderDate,
        //        TotalAmount = order.TotalAmount,
        //        Status = order.Status,
        //        Items = orderItems.Select(i => new OrderItemDto
        //            {
        //            ProductId = i.ProductId,
        //            Name = i.Name,
        //            Quantity = i.Quantity,
        //            Price = i.Price
        //            }).ToList()
        //        };

        //    return Result<object>.Ok(orderDto, "Checkout successful", 200);
        //    }

        //public async Task<Result<object>> CheckOutAsync(string userId)
        //    {
        //    if (string.IsNullOrEmpty(userId))
        //        return Result<object>.Fail(null, "User not logged in", 401);

        //    var cacheKey = $"cart_{userId}";
        //    Cart? cart = null;

        //    // Try to get cart from Redis cache
        //    var cache = await _cache.GetStringAsync(cacheKey);
        //    if (!string.IsNullOrWhiteSpace(cache))
        //        {
        //        cart = System.Text.Json.JsonSerializer.Deserialize<Cart>(cache, new System.Text.Json.JsonSerializerOptions
        //            {
        //            PropertyNameCaseInsensitive = true
        //            });
        //        }
        //    else
        //        {
        //        var cartsFromDb = await _context.GetUserCartAsync(userId);
        //        if (cartsFromDb != null && cartsFromDb.Any())
        //            {
        //            cart = cartsFromDb.First(); // Assuming one cart per user
        //            await _cache.SetStringAsync(cacheKey,
        //                System.Text.Json.JsonSerializer.Serialize(cart),
        //                new DistributedCacheEntryOptions
        //                    {
        //                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10)
        //                    });
        //            }
        //        }

        //    if (cart == null || cart.Products == null || !cart.Products.Any())
        //        return Result<object>.Fail(null, "Your cart is empty", 400);

        //    var totalAmount = cart.Products.Sum(p => p.Price * p.Quantity);

        //    var order = new Order
        //        {
        //        UserId = userId,
        //        Name = "Checkout Order",
        //        OrderDate = DateTime.UtcNow,
        //        TotalAmount = totalAmount,
        //        Status = "Pending"
        //        };

        //    var orderItems = cart.Products.Select(p => new OrderItem
        //        {
        //        ProductId = p.ProductId,
        //        Name = string.IsNullOrEmpty(p.Name) ? "Unknown Product" : p.Name,
        //        Quantity = p.Quantity,
        //        Price = p.Price
        //        }).ToList();

        //    await _context.SaveOrderAsync(order, orderItems);

        //    // Clear cart from DB and Redis
        //    await _context.ClearCartAsync(new List<Cart> { cart });
        //    await _cache.RemoveAsync(cacheKey);

        //    var orderDto = new OrderDto
        //        {
        //        OrderId = order.OrderId,
        //        UserId = order.UserId,
        //        OrderDate = order.OrderDate,
        //        TotalAmount = order.TotalAmount,
        //        Status = order.Status,
        //        Items = orderItems.Select(i => new OrderItemDto
        //            {
        //            ProductId = i.ProductId,
        //            Name = i.Name,
        //            Quantity = i.Quantity,
        //            Price = i.Price
        //            }).ToList()
        //        };

        //    return Result<object>.Ok(orderDto, "Checkout successful", 200);
        //    }
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

        public async Task<Order?> GetLatestOrderAsync(string userId)
            {
            var result= await _context.GetLatestOrderAsync(userId);
            return result;
            }

        public async Task<(bool success, string message, object data)> GetOrderDetailsAsync(string userId)
            {
            if (string.IsNullOrWhiteSpace(userId))
                return (false, "Invalid UserId.", null);

            var order = await _context.GetLatestOrderAsync(userId);

            if (order == null)
                return (false, "No orders found for this user.", null);

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

            return (true, "Order details fetched successfully.", orderDto);
            }

        }
    }
