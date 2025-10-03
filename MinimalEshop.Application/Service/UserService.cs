using MinimalEshop.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MinimalEshop.Application.Interface;

namespace MinimalEshop.Application.Service
{
    public class UserService : IUser
    {
        private readonly EshopDbcontext _context;
        public RegisterUserService(EshopDbcontext context)
        {
            _context = context;
        }
        public async Task<User> RegisterUserAsync(string username, string password, string email, string role)
        {
            var user = new User
            {
                Username = username,
                Password = password,
                Email = email,
                Role = role
            };
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return user;
        }
    }

}
