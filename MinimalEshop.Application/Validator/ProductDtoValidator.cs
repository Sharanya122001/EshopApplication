using FluentValidation;
using MinimalEshop.Application.DTO;

namespace MinimalEshop.Application.Validator
{
    public class ProductDtoValidator : AbstractValidator<ProductDto>
    {
        public ProductDtoValidator()
        {
            RuleFor(x => x.ProductId)
                .NotEmpty().WithMessage("ProductId Required")
                .Length(24).WithMessage("ProductId must be of 24 character");
        }
    }
}
