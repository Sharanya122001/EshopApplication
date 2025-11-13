using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using MinimalEshop.Application.Domain.Entities;
using MinimalEshop.Application.DTO;
using MinimalEshop.Application.Service;
using MinimalEshop.Presentation.Responses;
using System.ComponentModel.DataAnnotations;

namespace MinimalEshop.Presentation.RouteGroup
    {
    public static class ProductRouteGroup
        {
        public static RouteGroupBuilder ProductAPI(this RouteGroupBuilder group)
            {

            group.MapGet("/", async ([FromServices] ProductService _service) =>
            {
                var product = await _service.GetProductAsync();
                return Results.Ok(Result.Ok(product, null, StatusCodes.Status200OK));
            })
            .RequireAuthorization("UserOrAdmin")
            .WithTags("Product");

            group.MapGet("/search", async ([FromServices] ProductService _service, [FromQuery] string query) =>
            {
                if (string.IsNullOrWhiteSpace(query))
                    return Results.BadRequest(Result.Fail(null, "Query cannot be empty", StatusCodes.Status400BadRequest));

                var results = await _service.SearchProductsAsync(query);
                return Results.Ok(Result.Ok(results, null, StatusCodes.Status200OK));
            }).RequireAuthorization("UserOrAdmin")
            .WithTags("Product");

            group.MapPost("/", async ([FromServices] ProductService _service, [FromBody] ProductDto productDto) =>
            {
                var product = new Product
                    {
                    Name = productDto.Name,
                    Price = productDto.Price,
                    Description = productDto.Description,
                    CategoryId = productDto.CategoryId,
                    Addedon = productDto.Addedon
                    };
                var created = await _service.CreateProductAsync(product);
                return Results.Ok(Result.Ok(created, "Product created", StatusCodes.Status201Created));
            }).RequireAuthorization("AdminOnly")
            .WithTags("Product");

            group.MapPut("/update", async ([FromServices] ProductService _service, [FromBody] ProductDto productDto) =>
            {
                var product = new Product
                    {
                    ProductId = productDto.ProductId,
                    Name = productDto.Name,
                    Price = productDto.Price,
                    Description = productDto.Description,
                    CategoryId = productDto.CategoryId,
                    Addedon = productDto.Addedon
                    };

                var updated = await _service.UpdateProductAsync(product);
                return Results.Ok(Result.Ok(updated, updated ? "Product updated" : "Product update failed", StatusCodes.Status200OK));
            }).RequireAuthorization("AdminOnly")
            .WithTags("Product");

            group.MapDelete("/delete", async ([FromServices] ProductService _service,[FromServices] IValidator<ProductDto> validator, [FromQuery] string ProductId) =>
            {
                var dto = new ProductDto
                    {
                    ProductId = ProductId
                    };
                var validationResult = await validator.ValidateAsync(dto);
                if (!validationResult.IsValid)
                    {
                    var errors = string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage));
                    return Results.BadRequest(Result.Fail(null, errors, StatusCodes.Status400BadRequest));
                    }
                var deleted = await _service.DeleteProductAsync(ProductId);
                return Results.Ok(Result.Ok(deleted, deleted ? "Product deleted" : "Product delete failed", StatusCodes.Status200OK));
            }).RequireAuthorization("AdminOnly")
            .WithTags("Product");

            return group;
            }
        }
    }
