using FluentValidation;
using FluentValidationExamples.Models;

namespace FluentValidationExamples.Validators
{
    public class OrderItemValidator : AbstractValidator<OrderItem>
    {
        public OrderItemValidator()
        {
            RuleFor(i => i.ProductCode)
                .NotEmpty()
                .Matches(@"^[A-Z]{3}-\d{4}$").WithMessage("Product code must look like ABC-1234.");

            RuleFor(i => i.Quantity).GreaterThan(0).LessThanOrEqualTo(100);

            RuleFor(i => i.UnitPrice).GreaterThan(0).PrecisionScale(10, 2, true);
        }
    }
}
