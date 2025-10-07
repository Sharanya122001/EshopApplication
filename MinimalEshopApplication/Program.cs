
using Microsoft.AspNetCore.Mvc;
using MinimalEshop.Application.DTO;
using MinimalEshop.Application.Interface;
using MinimalEshop.Application.Service;
using MinimalEshop.Domain.Entities;
using MinimalEshop.Infrastructure.DataBaseContext;
using MinimalEshop.Presentation.RouteGroup;
using System.ComponentModel;


namespace MinimalEshopApplication
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddAuthorization();

            // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
            builder.Services.AddSwaggerGen();
            builder.Services.AddScoped<IProduct, ProductService>();
            builder.Services.AddScoped<IUser, UserService>();
            builder.Services.AddScoped<IOrder, OrderService>();
            builder.Services.AddScoped<ICart, CartService>();

            builder.Services.AddDbContext<EshopDbcontext>();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();
            

            app.MapGet("/", () =>
            {
                return "hello world!";
            });
            
            var cartGroup = app.MapGroup("/cart").CartAPI();
            var productGroup = app.MapGroup("/products").ProductAPI();
            var userGroup = app.MapGroup("/users").UserAPI();
            var orderGroup = app.MapGroup("/orders").OrderAPI();

            app.Run();
        }
    }
}
