using MinimalEshop.Application.Domain.Entities;
using MinimalEshop.Application.DTO;

namespace MinimalEshop.Application.Interface
{
    public interface IUser
    {
        Task<User> RegisterAsync(User user);
        Task<User?> GetUserByUsernameAsync(string username);

    }
}
