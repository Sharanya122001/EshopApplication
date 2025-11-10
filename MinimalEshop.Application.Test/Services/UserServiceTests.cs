using AutoFixture;
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
        private readonly IFixture _fixture;

        public UserServiceTests()
            {
            _fixture = new Fixture();
            _userRepositoryMock = new Mock<IUser>();
            _tokenServiceMock = new Mock<ITokenService>();

            _userService = new UserService(_userRepositoryMock.Object, _tokenServiceMock.Object);
            }

        [Fact]
        public async Task RegisterUserAsync_ReturnsRegisteredUser()
            {
            var user = _fixture.Build<User>()
                               .With(u => u.Role, "Customer")
                               .Create();

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
            var user = _fixture.Build<User>()
                               .With(u => u.Role, "Customer")
                               .Create();

            _userRepositoryMock
                .Setup(repo => repo.GetUserByUsernameAsync(user.Username))
                .ReturnsAsync(user);

            var tokenValue = _fixture.Create<string>();
            _tokenServiceMock
                .Setup(ts => ts.GenerateToken(user.UserId, user.Username, user.Role))
                .Returns(tokenValue);

            var token = await _userService.LoginAsync(user.Username, user.Password);

            Assert.NotNull(token);
            Assert.Equal(tokenValue, token);

            _userRepositoryMock.Verify(repo => repo.GetUserByUsernameAsync(user.Username), Times.Once);
            _tokenServiceMock.Verify(ts => ts.GenerateToken(user.UserId, user.Username, user.Role), Times.Once);
            }

        [Fact]
        public async Task LoginAsync_WithInvalidUser_ReturnsNull()
            {
            var invalidUsername = _fixture.Create<string>();

            _userRepositoryMock
                .Setup(repo => repo.GetUserByUsernameAsync(invalidUsername))
                .ReturnsAsync((User?)null);

            var token = await _userService.LoginAsync(invalidUsername, _fixture.Create<string>());

            Assert.Null(token);
            }
        }
    }

