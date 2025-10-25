using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using MinimalEshop.Application.Domain.Entities;
using MinimalEshop.Application.DTO;
using MinimalEshop.Application.Service;

namespace MinimalEshop.Presentation.RouteGroup
{
    public static class UserRouteGroup
    {
        public static RouteGroupBuilder UserAPI(this RouteGroupBuilder group)
        {
            group.MapPost("/Register", async([FromServices] UserService _service, [FromBody] UserDto userDto) =>
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
            });

            return group;
        }
    }
}
