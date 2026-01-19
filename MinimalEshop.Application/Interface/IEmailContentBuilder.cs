using MinimalEshop.Application.Domain.Entities;
using MinimalEshop.Application.DTO;

public interface IEmailContentBuilder
{
    EmailMessageDto BuildOrderPlacedEmail(
        User user,
        Order order,
        List<OrderItem> items
    );

    EmailMessageDto BuildPaymentSuccessEmail(
        User user,
        Order order,
        List<OrderItem> items
    );
    EmailMessageDto BuildPaymentFailedEmail(
       User user,
       Order order,
       List<OrderItem> items,
       string? failureReason = null
   );

}
