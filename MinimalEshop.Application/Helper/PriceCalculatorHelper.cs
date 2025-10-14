using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
