using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinimalEshop.Application.DTO
{
    public class AuthDtos
    {
        public record RegisterRequest(string Username, string Password, string Role); 
        public record LoginRequest(string Username, string Password);
        public record LoginResponse(string Token, DateTime Expires);
    }
}
