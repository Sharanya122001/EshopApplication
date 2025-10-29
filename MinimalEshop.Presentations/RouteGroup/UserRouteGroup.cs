using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using MinimalEshop.Application.Domain.Entities;
using MinimalEshop.Application.DTO;
using MinimalEshop.Application.Service;
using System.Security.Claims;

namespace MinimalEshop.Presentation.RouteGroup
{
    public static class UserRouteGroup
    {
        public static RouteGroupBuilder UserAPI(this RouteGroupBuilder group)
        {
            group.MapPost("/Register", async ([FromServices] UserService _service, [FromBody] UserDto userDto) =>
            {
                var user = new User
                {
                    Username = userDto.Username,
                    Password = userDto.Password,
                    Email = userDto.Email,
                    Role = userDto.Role
                };
                var registered = await _service.RegisterUserAsync(user.Username, user.Password, user.Email, user.Role);
                return registered;
            })
            .WithTags("User");

            group.MapPost("/Login", async ([FromServices] UserService _service,[FromBody] LoginDto loginDto) =>
            {
                if (loginDto == null)
                    return Results.BadRequest("Invalid request body.");

                var token = await _service.LoginAsync(loginDto.Username, loginDto.Password);

                if (token == null)
                    return Results.Unauthorized();

                return Results.Ok(new { Token = token });

            })
            .WithTags("User");

            return group;
        }
    }
}
