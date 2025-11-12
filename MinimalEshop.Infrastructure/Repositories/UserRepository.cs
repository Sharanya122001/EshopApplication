using Microsoft.EntityFrameworkCore;
using MinimalEshop.Application.Domain.Entities;
using MinimalEshop.Application.Interface;
using MinimalEshop.Infrastructure.Context;
using MongoDB.Driver;

namespace MinimalEshop.Infrastructure.Repositories
    {
    public class UserRepository : IUser
        {
        private readonly MongoDbContext _context;

        public UserRepository(MongoDbContext context)
            {
            _context = context;
            }
        public async Task<User> RegisterAsync(User user)
            {
            await _context.AddAsync(user);
            await _context.SaveChangesAsync();
            return user;
            }

        public async Task<User?> GetUserByUsernameAsync(string username)
            {
            return await _context.Users.Where(u => u.Username == username).FirstOrDefaultAsync();
            }
        }
    }
