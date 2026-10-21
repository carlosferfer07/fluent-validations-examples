using FluentValidation;
using FluentValidationExamples.Models;

namespace FluentValidationExamples.Validators
{
    public class AddressValidator : AbstractValidator<Address>
    {
        public AddressValidator()
        {
            RuleFor(a => a.Street).NotEmpty().MaximumLength(200);
            RuleFor(a => a.City).NotEmpty().MaximumLength(100);
            RuleFor(a => a.Country).NotEmpty().Length(2).WithMessage("Use the 2 letter country code (e.g. PT, ES, US).");

            // Portuguese postal codes look like 1000-001
            RuleFor(a => a.PostalCode)
                .NotEmpty()
                .Matches(@"^\d{4}-\d{3}$").When(a => a.Country == "PT", ApplyConditionTo.CurrentValidator)
                .WithMessage("Portuguese postal codes must look like 1234-567.");
        }
    }
}
