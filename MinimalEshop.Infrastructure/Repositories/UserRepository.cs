using MinimalEshop.Application.Domain.Entities;
using MinimalEshop.Application.Interface;
using MinimalEshop.Infrastructure.Context;
using MongoDB.Driver;

namespace MinimalEshop.Infrastructure.Repositories
    {
    public class UserRepository : IUser
        {
        private readonly IMongoCollection<User> _users;

        public UserRepository(MongoDbContext context)
            {
            _users = context.Users;
            }
        public async Task<User> RegisterAsync(User user)
            {
            await _users.InsertOneAsync(user);
            return user;
            }

        public async Task<User?> GetUserByUsernameAsync(string username)
            {
            return await _users.Find(u => u.Username == username).FirstOrDefaultAsync();
            }
        }
    }
