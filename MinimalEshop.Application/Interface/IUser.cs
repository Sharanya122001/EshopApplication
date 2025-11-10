using MinimalEshop.Application.Domain.Entities;

namespace MinimalEshop.Application.Interface
    {
    public interface IUser
        {
        Task<User> RegisterAsync(User user);
        Task<User?> GetUserByUsernameAsync(string username);

        }
    }
