using CorrelationId.DependencyInjection;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using MinimalEshop.Application.Interface;
using MinimalEshop.Application.Service;
using MinimalEshop.Application.Validator;
using MinimalEshop.Infrastructure.Data;
using MinimalEshop.Infrastructure.Repositories;
using MinimalEshop.Presentation;
using MinimalEshop.Presentation.Responses;
using MinimalEshop.Presentation.RouteGroup;
using MongoDB.Driver;
using Serilog;
using System.Reflection;
using System.Text;
using CorrelationId;




namespace Presentation
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            //string stripeApiUrl = Environment.GetEnvironmentVariable("STRIPE_API_URL") ?? "http://localhost:5001/api/payment";
            builder.Services.AddHttpClient("StripeDemo", c =>
            {
                c.BaseAddress = new Uri("https://localhost:44325/");
            });

            //registering the httpclient i.e., to call stripe api
            builder.Services.AddHttpClient("StripeDemo", c =>
            {
                c.BaseAddress = new Uri("https://localhost:44325/");
            });


            builder.Host.UseSerilog((context, services, loggerConfig) =>
            {
                loggerConfig
                    .ReadFrom.Configuration(context.Configuration)
                    .ReadFrom.Services(services)
                    .Enrich.FromLogContext();
            });

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

            //configuring mongoDb with EFcore
            builder.Services.AddDbContext<MinimalEshop.Infrastructure.Context.MongoDbContext>(options =>
            {
                options.UseMongoDB("mongodb+srv://Sharanya:Sharanya@cluster0.m2cqpvh.mongodb.net/", "MinimalEshopDB");//usemongo takes 2 arguments one is connectionstring and second is database name
            });

            builder.Services.AddScoped<IUserRepo, UserRepository>();
            builder.Services.AddScoped<IProductRepo, ProductRepository>();
            builder.Services.AddScoped<IOrderRepo, OrderRepository>();
            builder.Services.AddScoped<ICartRepo, CartRepository>();

            builder.Services.AddScoped<UserService>();
            builder.Services.AddScoped<ProductService>();
            builder.Services.AddScoped<OrderService>();
            builder.Services.AddScoped<CartService>();
            builder.Services.AddScoped<CategoryService>();

            builder.Services.AddValidatorsFromAssemblyContaining<Program>();
            builder.Services.AddFluentValidationAutoValidation();
            builder.Services.AddValidatorsFromAssemblyContaining<Program>();
            builder.Services.AddValidatorsFromAssemblyContaining<LoginDtoValidation>();


            builder.Services.AddScoped<ITokenService, TokenService>();
            builder.Services.AddScoped<ICacheService, CacheService>();

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
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);

                options.IncludeXmlComments(xmlPath);
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
            //registered redis cache
            builder.Services.AddStackExchangeRedisCache(Options => { Options.Configuration = "redis-11198.crce263.ap-south-1-1.ec2.cloud.redislabs.com:11198,password=50wCLUHaUvPGMpf3l1QjH7ExjxRL0bZs"; Options.InstanceName = "MinimalEshopCacheInstance"; });
            builder.Services.AddDefaultCorrelationId(options =>
            {
                options.IncludeInResponse = true;
                options.UpdateTraceIdentifier = true;
            });

            var app = builder.Build();

            app.UseSwagger();
            app.UseSwaggerUI();

            app.UseSerilogRequestLogging();
            app.UseCorrelationId();

            app.UseAuthentication();
            app.UseAuthorization();
            app.UseMiddleware<AuthResponseMiddleware>();

            app.MapGroup("/cart").CartAPI();
            app.MapGroup("/products").ProductAPI();
            app.MapGroup("/users").UserAPI();
            app.MapGroup("/orders").OrderAPI();

            app.Run();

        }
    }
}

