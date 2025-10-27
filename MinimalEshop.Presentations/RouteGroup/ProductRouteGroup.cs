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
            //group.MapPost("/products", async () =>
            //{
            //    return "Product created";
            //})
            // .RequireAuthorization();

            group.MapGet("/", async ([FromServices] ProductService _service) =>
            {
                var product = await _service.GetProductAsync();
                return product;
            }).RequireAuthorization("UserOrAdmin");

            group.MapGet("/products/search", async ([FromServices] ProductService _service, [FromQuery] string query) =>
            {
                if (string.IsNullOrWhiteSpace(query))
                    return Results.BadRequest("Query cannot be empty");

                var results = await _service.SearchProductsAsync(query);
                return Results.Ok(results);
            }).RequireAuthorization("UserOrAdmin");

            //group.MapGet("/category/{categoryId}", async ([FromServices] ProductService _service, string categoryId) =>
            //{
            //    if (string.IsNullOrWhiteSpace(categoryId))
            //        return Results.BadRequest("Category ID cannot be empty");

            //    var products = await _service.GetProductsByCategoryAsync(categoryId);

            //    if (products == null || !products.Any())
            //        return Results.NotFound("No products found for the given category");

            //    return Results.Ok(products);
            //});



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
            }).RequireAuthorization("AdminOnly");

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
            }).RequireAuthorization("AdminOnly");

            group.MapDelete("/delete", async ([FromServices] ProductService _service, [FromQuery] string ProductId) =>
            {
                var deleted = await _service.DeleteProductAsync(ProductId);
                return Results.Ok(deleted);
            }).RequireAuthorization("AdminOnly");


            return group;
        }
    }
}
