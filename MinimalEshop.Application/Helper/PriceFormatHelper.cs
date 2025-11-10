namespace MinimalEshop.Application.Helper
    {
    public class PriceFormatHelper
        {
        public static decimal PriceFormat(decimal price)
            {
            return Math.Round(price, 2);
            }
        }
    }
