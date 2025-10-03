using MinimalEshop.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MinimalEshop.Application.Interface;

namespace MinimalEshop.Application.Service
{
    public class OrderService:IOrder
    {
        private readonly EshopDbcontext _context;
        public OrderService(EshopDbcontext context)
        {
            _context = context;
        }

        public async Task<bool> CheckOutAsync(int UserId)
        {
            var cart=await _context.Cart
                .where(c=>c.UserId==userId)
                .Incude(c=>c.Product)
                .ToListAsync();

            if(!cart.Any()) return false;

            var order = new Order
            {
                UserId = UserId,
                OrderDate = DateTime.Now,
                TotalAmount=cart.Sum(c=>c.Product.Price*c.Quantity)
                .Tolist()
            };
            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            var orderItems = cart.Select(c => new OrderItem
            {
                OrderId = order.OrderId,
                ProductId=c.ProductId,
                Quantity=c.Quantity,
                Price=c.Product.Price

            }).ToList();
            
            _context.Carts.RemoveRange(cart);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<PaymentStatus> ProcessPaymentAsync(int OrderId)
        {
            var order = await _context.Orders.FindAsync(OrderId);
            if (order = null) return PaymentStatus.Failed;

            var paymentMethod = order.PaymentMethod;

            switch (paymentMethod)
            {
                case PaymentMethod.CashOnDelivery:
                    return PaymentStatus.Pending;
                case PaymentMethod.UPI:
                case PaymentMethod.NetBanking:
                case PaymentMethod.Card:
                    return PaymentStatus.Success;
                default:
                    return PaymentStatus.Failed;
            }
        }

        public async Task<OrderItem> CheckOrderDetailsAsync(int OrderId)
        {
            var orderItem= await _context.Orders.FindaAsync(OrderId);
            return orderItem;

        }


    }
}
