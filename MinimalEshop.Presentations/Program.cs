using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using MinimalEshop.Application.Interface;
using MinimalEshop.Application.Service;
using MinimalEshop.Application.Validator;
using MinimalEshop.Infrastructure.Context;
using MinimalEshop.Infrastructure.Data;
using MinimalEshop.Infrastructure.Repositories;
using MinimalEshop.Presentation;
using MinimalEshop.Presentation.Responses;
using MinimalEshop.Presentation.RouteGroup;
using MongoDB.Driver;
using MongoFramework;
using System.Text;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;


namespace Presentation
    {
    public class Program
        {
        public static void Main(string[] args)
            {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.Configure<JwtSettings>(
                builder.Configuration.GetSection("Jwt")
            );
            builder.Services.Configure<MongoDBSettings>(
                builder.Configuration.GetSection("MongoDBSettings")
            );

            builder.Services.AddSingleton<IMongoClient>(s =>
            {
                var settings = s.GetRequiredService<IOptions<MongoDBSettings>>().Value;
                return new MongoClient(settings.ConnectionString);
            });

            builder.Services.AddDbContext<MinimalEshop.Infrastructure.Context.MongoDbContext>(options =>
            {
                options.UseMongoDB("mongodb+srv://Sharanya:Sharanya@cluster0.m2cqpvh.mongodb.net/", "MinimalEshopDB");//usemongo takes 2 arguments one is connectionstring and second is database name
            });

            builder.Services.AddScoped<IUser, UserRepository>();
            builder.Services.AddScoped<IProduct, ProductRepository>();
            builder.Services.AddScoped<IOrder, OrderRepository>();
            builder.Services.AddScoped<ICart, CartRepository>();

            builder.Services.AddScoped<UserService>();
            builder.Services.AddScoped<ProductService>();
            builder.Services.AddScoped<OrderService>();
            builder.Services.AddScoped<CartService>();
            builder.Services.AddScoped<CategoryService>();

            builder.Services.AddControllers()
              .AddFluentValidation(fv =>
              {
                  fv.RegisterValidatorsFromAssemblyContaining<UserDtoValidator>();
              });

            builder.Services.AddScoped<ITokenService, TokenService>();

            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                var jwtSettings = builder.Configuration.GetSection("Jwt").Get<JwtSettings>();

                if (jwtSettings == null || string.IsNullOrEmpty(jwtSettings.Key))
                    {
                    throw new InvalidOperationException("JWT settings are not configured. Ensure 'Jwt' section exists with a non-empty 'Key'.");
                    }

                options.TokenValidationParameters = new TokenValidationParameters
                    {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(jwtSettings.Key)
                    ),
                    ValidateIssuer = true,
                    ValidIssuer = jwtSettings.Issuer,
                    ValidateAudience = true,
                    ValidAudience = jwtSettings.Audience,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.FromSeconds(30)
                    };
            });

            builder.Services.AddAuthorization(options =>
            {
                options.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));
                options.AddPolicy("UserOrAdmin", policy => policy.RequireRole("User", "Admin"));
            });

            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo
                    {
                    Title = "MinimalEshop API",
                    Version = "v1"
                    });

                var jwtSecurityScheme = new OpenApiSecurityScheme
                    {
                    Scheme = "bearer",
                    BearerFormat = "JWT",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.Http,
                    Description = "Enter 'Bearer' [space] and then your valid token.",
                    Reference = new OpenApiReference
                        {
                        Id = JwtBearerDefaults.AuthenticationScheme,
                        Type = ReferenceType.SecurityScheme
                        }
                    };

                options.AddSecurityDefinition(jwtSecurityScheme.Reference.Id, jwtSecurityScheme);
                options.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                   { jwtSecurityScheme, Array.Empty<string>() }
                });
            });

            var app = builder.Build();

            // Global exception handler
            app.UseExceptionHandler(errApp =>
            {
                errApp.Run(async context =>
                {
                    context.Response.ContentType = "application/json";

                    var feature = context.Features.Get<Microsoft.AspNetCore.Diagnostics.IExceptionHandlerFeature>();
                    var ex = feature?.Error;

                    var result = Result.Fail(new[] { ex?.Message ?? "An unexpected error occurred." }, "An error occurred while processing your request.", 500);

                    context.Response.StatusCode = 500;
                    await context.Response.WriteAsJsonAsync(result);
                });
            });

            if (app.Environment.IsDevelopment())
                {
                app.UseSwagger();
                app.UseSwaggerUI();
                }

            app.UseHttpsRedirection();

            app.Use(async (context, next) =>
            {
                await next();

                if (context.Response.StatusCode == StatusCodes.Status403Forbidden)
                    {
                    context.Response.ContentType = "application/json";
                    await context.Response.WriteAsJsonAsync(Result.Fail(null, "You are not authorized to access this resource.", StatusCodes.Status403Forbidden));
                    }
                else if (context.Response.StatusCode == StatusCodes.Status401Unauthorized)
                    {
                    context.Response.ContentType = "application/json";
                    await context.Response.WriteAsJsonAsync(Result.Fail(null, "Authentication is required.", StatusCodes.Status401Unauthorized));
                    }
            });

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapGroup("/cart").CartAPI();
            app.MapGroup("/products").ProductAPI();
            app.MapGroup("/users").UserAPI();
            app.MapGroup("/orders").OrderAPI();

            app.Run();

            }
        }
    }

