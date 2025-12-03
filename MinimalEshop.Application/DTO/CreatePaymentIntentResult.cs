using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinimalEshop.Application.DTO
    {
    public class CreatePaymentIntentResult
        {
        public string paymentIntentId { get; set; }
        public string clientSecret { get; set; }
        }
    }
