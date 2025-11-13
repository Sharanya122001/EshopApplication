using FluentValidation;
using MinimalEshop.Application.DTO;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinimalEshop.Application.Validator
    {
    public class CartDtoValidator : AbstractValidator<CartDto>
        {
        public CartDtoValidator()
            {
            RuleFor(x => x.ProductId)
                .NotEmpty().WithMessage("ProductId is required.")
                .Must(id => ObjectId.TryParse(id, out _))
                .WithMessage("Invalid ProductId format.");

            RuleFor(x => x.Quantity)
                .GreaterThan(0).WithMessage("Quantity must be greater than 0.");
            }

        }
    }
    
        
    
