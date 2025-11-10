using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using MinimalEshop.Application.Domain.Entities;
using MinimalEshop.Application.DTO;
using MinimalEshop.Application.Service;
using MinimalEshop.Presentation.Responses;

namespace MinimalEshop.Presentation.RouteGroup
    {
    public static class UserRouteGroup
        {
        public static RouteGroupBuilder UserAPI(this RouteGroupBuilder group)
            {
            group.MapPost("/Register", async ([FromServices] UserService _service, [FromServices] IValidator<UserDto> validator, [FromBody] UserDto userDto) =>
            {
                if (userDto == null)
                    return Results.BadRequest(Result.Fail(null, "Invalid request body.", StatusCodes.Status400BadRequest));

                var validationResult = await validator.ValidateAsync(userDto);
                if (!validationResult.IsValid)
                    {
                    var errors = string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage));
                    return Results.BadRequest(Result.Fail(null, errors, StatusCodes.Status400BadRequest));
                    }

                var user = new User
                    {
                    Username = userDto.Username,
                    Password = userDto.Password,
                    Email = userDto.Email,
                    Role = userDto.Role
                    };
                var registered = await _service.RegisterUserAsync(user.Username, user.Password, user.Email, user.Role);
                if (registered != null)
                    return Results.Created($"/users/{registered.UserId}", Result.Ok(registered, "User registered successfully.", StatusCodes.Status201Created));

                return Results.BadRequest(Result.Fail(null, "User registration failed.", StatusCodes.Status400BadRequest));
            })
            .WithTags("User");

            group.MapPost("/Login", async ([FromServices] UserService _service, [FromBody] LoginDto loginDto) =>
            {
                if (loginDto == null)
                    return Results.BadRequest(Result.Fail(null, "Invalid request body.", StatusCodes.Status400BadRequest));

                var token = await _service.LoginAsync(loginDto.Username, loginDto.Password);

                if (token == null)
                    return Results.Json(Result.Fail(null, "Invalid username or password.", StatusCodes.Status401Unauthorized), statusCode: StatusCodes.Status401Unauthorized);

                return Results.Ok(Result.Ok(new { Token = token }, null, StatusCodes.Status200OK));

            })
            .WithTags("User");

            return group;
            }
        }
    }
