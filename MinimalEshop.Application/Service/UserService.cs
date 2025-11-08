using MinimalEshop.Application.Domain.Entities;
using MinimalEshop.Application.DTO;
using MinimalEshop.Application.Interface;
using MinimalEshop.Application.Validator;
using System.ComponentModel.DataAnnotations;
using FluentValidation;
using FluentValidation.Results;

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
