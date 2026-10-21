using FluentValidation;
using FluentValidationExamples.Models;

namespace FluentValidationExamples.Validators
{
    public class OrderValidator : AbstractValidator<Order>
    {
        public OrderValidator()
        {
            RuleFor(o => o.CustomerEmail).NotEmpty().EmailAddress();

            RuleFor(o => o.ShippingAddress)
                .NotNull()
                .SetValidator(new AddressValidator());

            RuleFor(o => o.Items)
                .NotEmpty().WithMessage("An order needs at least one item.");

            RuleForEach(o => o.Items)
                .SetValidator(new OrderItemValidator());

            RuleFor(o => o.Items)
                .Must(items => items.Select(i => i.ProductCode).Distinct().Count() == items.Count)
                .WithMessage("The same product shouldn't appear twice, update the quantity instead.");

            RuleFor(o => o.DiscountCode)
                .Must(code => code!.StartsWith("PROMO"))
                .WithMessage("Invalid discount code.")
                .When(o => !string.IsNullOrEmpty(o.DiscountCode));

            RuleFor(o => o.Total)
                .GreaterThanOrEqualTo(20)
                .WithMessage("Discount codes only work on orders of 20 or more (current total: {PropertyValue}).")
                .When(o => !string.IsNullOrEmpty(o.DiscountCode));
        }
    }
}
