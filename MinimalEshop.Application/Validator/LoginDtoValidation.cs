using FluentValidation;
using MinimalEshop.Application.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinimalEshop.Application.Validator
    {
    public class LoginDtoValidation : AbstractValidator<LoginDto>
        {
        public LoginDtoValidation()
            {
            RuleFor(x => x.Username)
                   .NotEmpty().WithMessage("Username is Required.");

            RuleFor(x => x.Password)
                   .NotEmpty().WithMessage("Password is Required")
                   .MinimumLength(6).WithMessage("Password must be at least 6 characters long.");

            }
        }
    }
