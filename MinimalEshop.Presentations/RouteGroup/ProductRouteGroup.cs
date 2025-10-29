using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using MinimalEshop.Application.Domain.Entities;
using MinimalEshop.Application.DTO;
using MinimalEshop.Application.Service;

namespace MinimalEshop.Presentation.RouteGroup
{
    public static class ProductRouteGroup
    {
        public static RouteGroupBuilder ProductAPI(this RouteGroupBuilder group)
        {

            group.MapGet("/", async ([FromServices] ProductService _service) =>
            {
                var product = await _service.GetProductAsync();
                return product;
            }).RequireAuthorization("UserOrAdmin")
            .WithTags("Product");

            group.MapGet("/products/search", async ([FromServices] ProductService _service, [FromQuery] string query) =>
            {
                if (string.IsNullOrWhiteSpace(query))
                    return Results.BadRequest("Query cannot be empty");

                var results = await _service.SearchProductsAsync(query);
                return Results.Ok(results);
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
                return created;
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
                return Results.Ok(updated);
            }).RequireAuthorization("AdminOnly")
            .WithTags("Product");

            group.MapDelete("/delete", async ([FromServices] ProductService _service, [FromQuery] string ProductId) =>
            {
                var deleted = await _service.DeleteProductAsync(ProductId);
                return Results.Ok(deleted);
            }).RequireAuthorization("AdminOnly")
            .WithTags("Product");

            return group;
        }
    }
}
