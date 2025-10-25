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
            });

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
            });

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
            });

            group.MapDelete("/delete", async ([FromServices] ProductService _service, [FromQuery] string ProductId) =>
            {
                var deleted = await _service.DeleteProductAsync(ProductId);
                return Results.Ok(deleted);
            });


            return group;
        }
    }
}
