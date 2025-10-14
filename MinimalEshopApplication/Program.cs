using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using MinimalEshop.Application.Domain.Entities;
using MinimalEshop.Application.DTO;
using MinimalEshop.Application.Interface;
using MinimalEshop.Application.Service;
using MinimalEshop.Infrastructure;
using MinimalEshop.Infrastructure.Context;
using MinimalEshop.Infrastructure.Data;
using MinimalEshop.Infrastructure.Repositories;
using MinimalEshop.Presentation.RouteGroup;
using MongoDB.Driver;
using System.ComponentModel;


namespace MinimalEshopApplication
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

           
            builder.Services.AddAuthorization();

 
            builder.Services.Configure<MongoDBSettings>(
                builder.Configuration.GetSection("MongoDBSettings"));

            builder.Services.AddSingleton<IMongoClient>(s =>
            {
                var settings = s.GetRequiredService<IOptions<MongoDBSettings>>().Value;
                return new MongoClient(settings.ConnectionString);
            });


            builder.Services.AddScoped<MongoDbContext>();

            // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
            builder.Services.AddSwaggerGen();
            builder.Services.AddScoped<IProduct, ProductRepository>();
            builder.Services.AddScoped<IUser, UserRepository>();
            builder.Services.AddScoped<IOrder, OrderRepository>();
            builder.Services.AddScoped<ICart, CartRepository>();

            builder.Services.AddScoped<ProductService>();
            builder.Services.AddScoped<CartService>();
            builder.Services.AddScoped<OrderService>();
            builder.Services.AddScoped<CategoryService>();
            builder.Services.AddScoped<UserService>();
            var app = builder.Build();
            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            var cartGroup = app.MapGroup("/cart").CartAPI();
            var productGroup = app.MapGroup("/products").ProductAPI();
            var userGroup = app.MapGroup("/users").UserAPI();
            var orderGroup = app.MapGroup("/orders").OrderAPI();

            app.Run();
        }
    }
}

