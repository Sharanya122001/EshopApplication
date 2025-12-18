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
            group.MapPost("/", async (
                ClaimsPrincipal user, 
                [FromServices] OrderService _orderService) =>
            {
                var userId = user.FindFirstValue(ClaimTypes.NameIdentifier);

                var result = await _orderService.CheckOutAsync(userId);

                if (!result.Success)
                    return Results.BadRequest(result);

                return Results.Ok(result);

            })
            .RequireAuthorization("UserOrAdmin")
            .WithTags("Order");


            group.MapPost("/payment", async (
                PaymentRequest request,
                IHttpClientFactory httpClientFactory,
                ILoggerFactory loggerFactory,
                OrderService _orderService, 
                ClaimsPrincipal user) =>
            {
                var logger = loggerFactory.CreateLogger("PaymentProcess");
                logger.LogInformation("Payment processing started");

                var userId = user.FindFirstValue(ClaimTypes.NameIdentifier);
                var method = request.PaymentMethod.Trim().ToLower();

                if (method == "cod" || method == "cash on delivery")
                {
                    return Results.Ok(new { message = "Cash on Delivery order placed successfully" });
                }
                //mapping the user method to stripe method
                var stripeMethod = method switch
                {
                    "card" => "card",
                    "upi" => "upi",
                    "netbanking" => "netbanking",
                    _ => null//invalid , so returns error
                };

                if (stripeMethod is null)
                    return Results.BadRequest(new { message = "Invalid payment method" });

                var client = httpClientFactory.CreateClient("StripeDemo");

                var createResponse = await client.PostAsJsonAsync("/create-payment-intent", new//sends post req to stripeto paymentintend with payload
                {
                    Amount = request.Amount,
                    Currency = "inr",
                    PaymentMethodType = stripeMethod
                });

                if (!createResponse.IsSuccessStatusCode)
                    return Results.BadRequest(new { message = "Stripe PaymentIntent creation failed" });

                var createResult = await createResponse.Content.ReadFromJsonAsync<CreatePaymentIntentResult>();

                var confirmResponse = await client.PostAsJsonAsync("/confirm-payment-intent", new//confrims the paymentintent
                {
                    PaymentIntentId = createResult.paymentIntentId,
                    PaymentMethodId = request.PaymentMethodId
                });

                if (!confirmResponse.IsSuccessStatusCode)
                    return Results.BadRequest(new { message = "Stripe PaymentIntent confirmation failed" });
                //reads the confirmed paymentintent result
                var confirmResult = await confirmResponse.Content.ReadFromJsonAsync<ConfirmPaymentIntentResult>();
                var paymentMethodEnum = Enum.Parse<MinimalEshop.Application.Domain.Enums.PaymentMethod>(request.PaymentMethod, true);

                var dbResult = await _orderService.ProcessPaymentAsync(userId, paymentMethodEnum);

                if (!dbResult.Success)
                    return Results.BadRequest(new { message = dbResult.Message });
                return Results.Ok(new
                {
                    message = "Payment successful",
                    paymentIntentId = confirmResult.id,
                    stripeStatus = confirmResult.status
                });
            })
             .RequireAuthorization("UserOrAdmin")
             .WithTags("Order");

            group.MapGet("/", async (
                ClaimsPrincipal user,
                [FromServices] OrderService _orderService,
                ILoggerFactory loggerFactory) =>
            {
                var logger = loggerFactory.CreateLogger("OrderRouteLogger");
                logger.LogInformation("GET/ Getting Order details");

                var userId = user.FindFirstValue(ClaimTypes.NameIdentifier);

                if (string.IsNullOrEmpty(userId))
                    return Results.Unauthorized();

                var result = await _orderService.GetOrderDetailsAsync(userId);

                if (!result.Success)
                    return Results.BadRequest(Result.Fail(null, result.Message, StatusCodes.Status400BadRequest));

                logger.LogInformation("Retrieved Order details");

                return Results.Ok(Result.Ok(result.Data, result.Message, StatusCodes.Status200OK));
            })
            .RequireAuthorization("UserOrAdmin")
            .WithTags("Order");

            return group;
        }
    }
}
