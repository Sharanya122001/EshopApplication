namespace MinimalEshop.Application.Interface
    {
    public interface ITokenService
        {
        string GenerateToken(string userId, string username, string role);
        }
    }
