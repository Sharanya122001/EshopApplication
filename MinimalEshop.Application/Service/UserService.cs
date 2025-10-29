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
        private readonly ITokenService _jwtService;
        public UserService(IUser user, ITokenService jwtService)
        {
            _user = user;
           _jwtService = jwtService;

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
        public async Task<string?> LoginAsync(string username, string password)
            {
            var existingUser = await _user.GetUserByUsernameAsync(username);

            if (existingUser == null || existingUser.Password != password)
                return null;

            var token = _jwtService.GenerateToken(
                existingUser.UserId,
                existingUser.Username,
                existingUser.Role
            );

            return token;
            }

        }

}
