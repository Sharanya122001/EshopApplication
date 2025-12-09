using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinimalEshop.Application.DTO
    {
    public class OrderDetailsResult<T>
        {
        public bool Success { get; set; }
        public string Message { get; set; }
        public T? Data { get; set; }
        }

    }
