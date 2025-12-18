using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using MinimalEshop.Application.Domain.Entities;
using MinimalEshop.Application.DTO;
using MinimalEshop.Application.Service;
using MinimalEshop.Presentation.Responses;

namespace MinimalEshop.Presentation.RouteGroup
{
    public static class ProductRouteGroup
    {
        public static RouteGroupBuilder ProductAPI(this RouteGroupBuilder group)
        {

            group.MapGet("/", async (
                [FromServices] ProductService _productService, 
                ILoggerFactory loggerFactory) =>
            {
                var logger = loggerFactory.CreateLogger("ProductRouteLogger");
                logger.LogInformation("GET/ Product Called");

                var product = await _productService.GetProductAsync();

                logger.LogInformation("Retrieved {Count} products", product.Count());

                return Results.Ok(Result.Ok(product, "Products retrieved successfully", StatusCodes.Status200OK));

            })
            .RequireAuthorization("UserOrAdmin")
            .Produces<List<Product>>(StatusCodes.Status200OK)
            .WithTags("Product");

            group.MapGet("/search", async (
                [FromServices] ProductService _productService,
                [FromQuery] string query,
                ILoggerFactory loggerFactory) =>
            {
                var logger = loggerFactory.CreateLogger("ProductRouteLogger");
                logger.LogInformation("GET /products/search called with query = {Query}", query);

                var results = await _productService.SearchProductsAsync(query);

                logger.LogInformation("Search returned {Count} items", results.Count());

                return Results.Ok(Result.Ok(results, null, StatusCodes.Status200OK));
            })
            .RequireAuthorization("UserOrAdmin")
            .Produces<List<Product>>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .WithTags("Product");

            group.MapPost("/", async (
                [FromHeader(Name = "Idempotency-Key")] string idempotencyKey, 
                [FromServices] ProductService _productService,
                [FromBody] ProductDto productDto,
                ILoggerFactory loggerFactory) =>
            {
                var logger = loggerFactory.CreateLogger("ProductRouteLogger");
                logger.LogInformation("POST /products called to add product {Name}", productDto.Name);

                var product = new Product
                {
                    Name = productDto.Name,
                    Price = productDto.Price,
                    Description = productDto.Description,
                    CategoryId = productDto.CategoryId,
                    Addedon = productDto.Addedon
                };
                var created = await _productService.CreateProductAsync(idempotencyKey, product);

                logger.LogInformation("Product created Id = {ProductId}", created.ProductId);

                return Results.Ok(Result.Ok(created, "Product created", StatusCodes.Status201Created));
            })
            .RequireAuthorization("AdminOnly")
            .Produces<Product>(StatusCodes.Status201Created)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .WithTags("Product");

            group.MapPut("/", async (
                [FromServices] ProductService _productService, 
                [FromBody] ProductDto productDto,
                ILoggerFactory loggerFactory) =>
            {
                var logger = loggerFactory.CreateLogger("ProductRouteLogger");
                logger.LogInformation("PUT /products/update called for ProductId = {ProductId}", productDto.ProductId);

                var product = new Product
                {
                    ProductId = productDto.ProductId,
                    Name = productDto.Name,
                    Price = productDto.Price,
                    Description = productDto.Description,
                    CategoryId = productDto.CategoryId,
                    Addedon = productDto.Addedon
                };

                var updated = await _productService.UpdateProductAsync(product);

                logger.LogInformation(updated ? "Product {ProductId} updated successfully" : "Failed to update Product {ProductId}", productDto.ProductId);

                return Results.Ok(Result.Ok(updated, updated ? "Product updated" : "Product update failed", StatusCodes.Status200OK));
            })
            .RequireAuthorization("AdminOnly")
            .Produces<bool>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .WithTags("Product");

            group.MapDelete("/", async (
                [FromServices] ProductService _productService, 
                [FromServices] IValidator<ProductDto> validator, 
                [FromQuery] string ProductId, 
                ILoggerFactory loggerFactory) =>
            {
                var logger = loggerFactory.CreateLogger("ProductRouteLogger");
                logger.LogInformation("DELETE/product/delete called for the ProductId={ProductId}", ProductId);

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

                var deleted = await _productService.DeleteProductAsync(ProductId);

                logger.LogInformation(deleted ? "Deleted product {ProductId}" : "Failed to Delete {ProductId}", ProductId);

                return Results.Ok(Result.Ok(deleted, deleted ? "Product deleted" : "Product delete failed", StatusCodes.Status200OK));
            })
            .RequireAuthorization("AdminOnly")
            .Produces<bool>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .WithTags("Product");

            return group;
        }
    }
}
