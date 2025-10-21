using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
