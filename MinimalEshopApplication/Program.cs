
using Microsoft.AspNetCore.Mvc;
using MinimalEshop.Application.Interface;
using MinimalEshop.Application.Service;
using MinimalEshop.Domain.Entities;
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

            //List<Category> categories = new List<Category>();
            //app.MapGet("/categorys", () =>
            //{
            //    return categories;
            //});

            //app.MapPost("/Categorys", (Category category) =>
            //{
            //    categories.Add(category);
            //    return category;
            //});

            //app.MapPut("/categorys/{categoryId}", (int categoryId,[FromBody] Category category) =>
            //{
            //    int index = categories.FindIndex(a => a.CategoryId == categoryId);
            //    if(index<0)
            //    {
            //        return Results.NotFound();
            //    }
            //    categories[index]=category;
            //    return Results.Ok(category);    
            //});

            //app.MapDelete("/category/{categoryId}", (int categoryId, [FromBody]Category category) =>
            //{
            //    int count = categories.RemoveAll(a => a.CategoryId == categoryId);
            //    if (count > 0)
            //    {
            //        return Results.Ok();
            //    }
            //    return Results.NotFound();
            //});

           
            app.MapGet("/products", () =>
            {
                return products;
            });

            app.MapPost("/products", (Product product) =>
                {
                    products.Add(product);
                    return products;
                });

            app.MapPut("/products/{ProductId}", (int ProductId, [ FromBody ]Product product) =>
            {
                int index = products.FindIndex(a => a.ProductId == ProductId);
                if (index < 0)
                {
                    return Results.NotFound();
                }
                products[index] = product;
                return Results.Ok(product);
            });

            app.MapDelete("/products/{productId}", (int productId) =>
            {
                int index = products.RemoveAll(app => app.ProductId == productId);
                if (index > 0)
                {
                    return Results.Ok();
                }
                return Results.NotFound();
            });

            app.Run();
        }
    }
}
