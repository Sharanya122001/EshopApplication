using MinimalEshop.Application.DTO;
using MinimalEshop.Application.Service;
using MinimalEshop.Domain.Entities;

namespace MinimalEshop.Presentation.RouteGroup
{
    public static class ProductRouteGroup
    {
        public static RouteGroupBuilder ProductAPI(this RouteGroupBuilder group)
        {
            group.MapGet("/", async (ProductService _service) =>
            {
                var product = await _service.GetProductAsync();
                return product;
            });

            group.MapPost("/", async (ProductService _service, ProductDto productDto) =>
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



            group.MapPut("/{ProductId}", async (int ProductId, ProductDto productDto, ProductService _service) =>
            {
                var product = new Product
                {
                    Name = productDto.Name,
                    Price = productDto.Price,
                    Description = productDto.Description,
                    CategoryId = productDto.CategoryId,
                    Addedon = productDto.Addedon
                };
                var updated = await _service.UpdateProductAsync(ProductId);
                return Results.Ok(updated);
            });

            group.MapDelete("/{productId}", async (int ProductId, ProductDto productDto, ProductService _service) =>
            {
                var product = new Product
                {
                    Name = productDto.Name,
                    Price = productDto.Price,
                    Description = productDto.Description,
                    CategoryId = productDto.CategoryId,
                    Addedon = productDto.Addedon
                };
                var updated = await _service.DeleteProductAsync(ProductId);
                return Results.Ok(updated);
            });

            return group;
        }
    }
}
