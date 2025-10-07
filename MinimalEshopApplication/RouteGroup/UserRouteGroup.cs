using MinimalEshop.Application.DTO;
using MinimalEshop.Application.Service;
using MinimalEshop.Domain.Entities;

namespace MinimalEshop.Presentation.RouteGroup
{
    public static class UserRouteGroup
    {
        public static RouteGroupBuilder UserAPI(this RouteGroupBuilder group)
        {
            group.MapPost("/Register", async(UserService _service, UserDto userDto) =>
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
