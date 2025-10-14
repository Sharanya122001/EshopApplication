using MinimalEshop.Application.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MinimalEshop.Application.Interface;

namespace MinimalEshop.Application.Service
{
    public class UserService
    {
        private readonly IUser _user;
        public UserService(IUser user)
        {
            _user = user;
        }
        public async Task<User> RegisterUserAsync(string username, string password, string email, string role)
        {
            if (string.IsNullOrEmpty(username)) throw new ArgumentException("Username required");
            if (string.IsNullOrEmpty(password)) throw new ArgumentException("Password required");
            if (string.IsNullOrEmpty(email)) throw new ArgumentException("Email required");

            var user = new User
            {
                Username = username,
                Password = password,
                Email = email,
                Role = role
            };
            return await _user.RegisterAsync(user);
        }
    }

}
