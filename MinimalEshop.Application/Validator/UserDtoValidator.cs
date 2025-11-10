using FluentValidation;
using MinimalEshop.Application.DTO;

namespace MinimalEshop.Application.Validator
    {
    public class UserDtoValidator : AbstractValidator<UserDto>
        {
        public UserDtoValidator()
            {
            RuleFor(x => x.Username)
                .NotEmpty().WithMessage("Username is Required.");

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("Password is Required.")
                .MinimumLength(6).WithMessage("Password must be of atleast 6 characters.")
                .Matches("[A-Z]").WithMessage("Password must contain atleast 1 Uppercase letter.")
                .Matches("[a-z]").WithMessage("Password must contain at least one lowercase letter.")
                .Matches("[0-9]").WithMessage("Password must contain at least one number.")
                .Matches("[^a-zA-Z0-9]").WithMessage("Password must contain at least one special character."); ;

            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email is Required.")
                .EmailAddress().WithMessage("Invalid email format.");

            RuleFor(x => x.Role)
                .NotEmpty().WithMessage("Role Required.")
                .Must(role =>
                !string.IsNullOrWhiteSpace(role) &&
                (role.Equals("admin", StringComparison.OrdinalIgnoreCase) ||
                 role.Equals("user", StringComparison.OrdinalIgnoreCase))).WithMessage("Role must be either 'Admin' or 'User' (case insensitive).");

            }
        }
    }
