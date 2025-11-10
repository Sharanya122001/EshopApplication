namespace MinimalEshop.Application.Helper
    {
    public static class PriceCalculatorHelper
        {
        public static decimal TotalAmount(decimal price, int quantity)
            {
            return price * quantity;
            }
        }
    }
