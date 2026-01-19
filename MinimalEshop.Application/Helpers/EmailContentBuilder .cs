using MinimalEshop.Application.Domain.Entities;
using MinimalEshop.Application.DTO;
using System.Text;

namespace MinimalEshop.Application.Helpers
{
    public class EmailContentBuilder : IEmailContentBuilder
    {
        public EmailMessageDto BuildOrderPlacedEmail(
            User user,
            Order order,
            List<OrderItem> items)
        {
            var userName = user?.Username ?? "Customer";
            var itemsText = BuildItemsText(items);

            var body =
                "ORDER PLACED SUCCESSFULLY\n\n" +
                $"Hi {userName},\n\n" +
                "Thank you for placing your order with us!\n\n" +

                "ORDER DETAILS\n" +
                $"Order Number   : {order.OrderNumber}\n" +
                $"Order Date UTC : {order.OrderDate:dd MMM yyyy, hh:mm tt}\n" +
                $"Order Status   : {order.Status}\n" +
                $"Payment Status : {order.PaymentStatus}\n" +
                $"Total Amount   : ₹{order.TotalAmount:N2}\n\n" +

                "ITEMS IN YOUR ORDER\n" +
                itemsText + "\n\n" +

                "WHAT HAPPENS NEXT?\n" +
                "- Complete the payment to start order processing\n\n" +

                "Thank you for shopping with us!\n" +
                "Minimal Eshop Team";

            return new EmailMessageDto
            {
                To = user.Email,
                Subject = "Order Placed Successfully",
                Body = body
            };
        }

        public EmailMessageDto BuildPaymentSuccessEmail(
            User user,
            Order order,
            List<OrderItem> items)
        {
            var userName = user?.Username ?? "Customer";
            var itemsText = BuildItemsText(items);

            var body =
                "PAYMENT SUCCESSFUL\n\n" +
                $"Hi {userName},\n\n" +
                "Your payment has been successfully processed.\n\n" +

                "ORDER DETAILS\n" +
                $"Order Number   : {order.OrderNumber}\n" +
                $"Payment Method : {order.PaymentMethod}\n" +
                $"Total Amount   : ₹{order.TotalAmount:N2}\n\n" +

                "ITEMS IN YOUR ORDER\n" +
                itemsText + "\n\n" +

                "WHAT HAPPENS NEXT?\n" +
                "- Your order is now being processed\n\n" +

                "Thank you for choosing us!\n" +
                "Minimal Eshop Team";

            return new EmailMessageDto
            {
                To = user.Email,
                Subject = "Payment Successful",
                Body = body
            };
        }

        public EmailMessageDto BuildPaymentFailedEmail(
    User user,
    Order order,
    List<OrderItem> items,
    string? failureReason = null)
        {
            var userName = user?.Username ?? "Customer";
            var itemsText = BuildItemsText(items);

            var body =
                "PAYMENT FAILED\n\n" +

                $"Hi {userName},\n\n" +

                "Unfortunately, we were unable to process your payment.\n\n" +

                "ORDER DETAILS\n" +
                $"Order Number   : {order.OrderNumber}\n" +
                $"Order Date UTC : {order.OrderDate:dd MMM yyyy, hh:mm tt}\n" +
                $"Payment Method : {order.PaymentMethod}\n" +
                $"Total Amount   : ₹{order.TotalAmount:N2}\n" +
                $"Payment Status : Failed\n\n" +

                (string.IsNullOrWhiteSpace(failureReason)
                    ? string.Empty
                    : $"FAILURE REASON\n{failureReason}\n\n") +

                "ITEMS IN YOUR ORDER\n" +
                itemsText + "\n\n" +

                "WHAT YOU CAN DO NEXT\n" +
                "- Retry the payment from your account\n" +
                "- Choose a different payment method\n" +
                "- Contact support if the issue persists\n\n" +

                "Your order has not been cancelled and will remain pending until payment is completed.\n\n" +

                "We’re here to help!\n" +
                "Minimal Eshop Team";

            return new EmailMessageDto
            {
                To = user.Email,
                Subject = "Payment Failed – Action Required",
                Body = body
            };
        }

        private string BuildItemsText(List<OrderItem> items)
        {
            var sb = new StringBuilder();

            foreach (var i in items)
            {
                sb.AppendLine(i.Name);
                sb.AppendLine($"  Quantity : {i.Quantity}");
                sb.AppendLine($"  Price    : ₹{i.Price:N2}");
                sb.AppendLine($"  Subtotal : ₹{i.Price * i.Quantity:N2}");
                sb.AppendLine();
            }

            return sb.ToString();
        }


    }
}
