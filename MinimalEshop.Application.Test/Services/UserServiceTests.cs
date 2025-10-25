using MinimalEshop.Application.Domain.Entities;
using MinimalEshop.Application.Interface;
using MinimalEshop.Application.Service;
using Moq;
using Xunit;

namespace MinimalEshop.Application.Test.Services
    {
    public class UserServiceTests
        {
        private readonly UserService _userService;
        private readonly Mock<IUser> _userRepositoryMock;
        private readonly Mock<ITokenService> _tokenServiceMock;

        public UserServiceTests()
            {
            _userRepositoryMock = new Mock<IUser>();
            _tokenServiceMock = new Mock<ITokenService>();

            _userService = new UserService(_userRepositoryMock.Object, _tokenServiceMock.Object);
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

            _userRepositoryMock
                .Setup(repo => repo.RegisterAsync(It.IsAny<User>()))
                .ReturnsAsync((User u) => u);
            var result = await _userService.RegisterUserAsync(
                user.Username, user.Password, user.Email, user.Role);
            Assert.NotNull(result);
            Assert.Equal(user.Username, result.Username);
            Assert.Equal(user.Email, result.Email);
            Assert.Equal(user.Role, result.Role);

            _userRepositoryMock.Verify(repo => repo.RegisterAsync(It.IsAny<User>()), Times.Once);
            }

        [Fact]
        public async Task LoginAsync_WithValidUser_ReturnsToken()
            {
            var username = "testuser";
            var password = "password123";
            var role = "Customer";

            var user = new User
                {
                Username = username,
                Password = password,
                Role = role
                };

            _userRepositoryMock
                .Setup(repo => repo.GetUserByUsernameAsync(username))
                .ReturnsAsync(user);

            _tokenServiceMock
                .Setup(ts => ts.GenerateToken(username, role))
                .Returns("dummy-token");

            var token = await _userService.LoginAsync(username, password);
            Assert.NotNull(token);
            Assert.Equal("dummy-token", token);

            _userRepositoryMock.Verify(repo => repo.GetUserByUsernameAsync(username), Times.Once);
            _tokenServiceMock.Verify(ts => ts.GenerateToken(username, role), Times.Once);
            }

        [Fact]
        public async Task LoginAsync_WithInvalidUser_ReturnsNull()
            {
            _userRepositoryMock
                .Setup(repo => repo.GetUserByUsernameAsync("invaliduser"))
                .ReturnsAsync((User?)null);
            var token = await _userService.LoginAsync("invaliduser", "password");

            Assert.Null(token);
            }
        }
    }

