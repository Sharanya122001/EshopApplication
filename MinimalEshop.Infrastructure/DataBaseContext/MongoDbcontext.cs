using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using MinimalEshop.Application.Domain.Entities;
using MinimalEshop.Infrastructure.Data;
using MongoDB.Driver;
using MongoDB.EntityFrameworkCore.Extensions;
using MongoFramework;

namespace MinimalEshop.Infrastructure.Context
    {
    public class MongoDbContext : DbContext
        {
        public DbSet<Product> Products { get; set; }
        public DbSet<Cart> Carts { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public MongoDbContext(DbContextOptions options) : base(options)
            {

            }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
            {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Product>().ToCollection("Products");
            modelBuilder.Entity<Category>().ToCollection("Categorie");
            modelBuilder.Entity<Cart>().ToCollection("Cart");
            modelBuilder.Entity<Order>().ToCollection("Order");
            modelBuilder.Entity<User>().ToCollection("User");
            modelBuilder.Entity<OrderItem>().ToCollection("OrderItem");

            }
        }
    }
