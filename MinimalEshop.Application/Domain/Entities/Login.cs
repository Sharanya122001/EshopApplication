using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinimalEshop.Application.Domain.Entities
{
    public class Login
    {
        public EmailAddressAttribute Email { get; set; }
        public string Password { get; set; }
    }
}
