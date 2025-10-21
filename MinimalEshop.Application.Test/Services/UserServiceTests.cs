using MinimalEshop.Application.Domain.Entities;
using MinimalEshop.Application.Interface;
using MinimalEshop.Application.Service;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinimalEshop.Application.Test.Services
{
    public class UserServiceTests
    {
        private readonly UserService _userService;
        private readonly Mock<IUser> _userRepositoryMock;

        public UserServiceTests()
        {
            _userRepositoryMock = new Mock<IUser>();
            _userService = new UserService(_userRepositoryMock.Object);
        }

        [Fact]

        public async Task RegisterUserAsync_ReturnsRegisteredUser()
        {
            var user = new User
            {
                Username = "testuser",
                Password = "password123",
                Email = "test@example.com",
                Role = "Customer"
            };
            _userRepositoryMock.Setup(repo => repo.RegisterAsync(user)).ReturnsAsync(user);

            var result=await _userService.RegisterUserAsync(user.Username, user.Password, user.Email, user.Role);   

            Assert.NotNull(result);
            Assert.Equal(user.Username, result.Username);
            Assert.Equal(user.Email, result.Email);
            Assert.Equal(user.Role, result.Role);

        }
    }
}
