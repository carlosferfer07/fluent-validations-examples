using FluentValidation;
using FluentValidationExamples.Models;

namespace FluentValidationExamples.Validators
{
    public class BookValidator : AbstractValidator<Book>
    {
        public BookValidator()
        {
            RuleFor(b => b.Name)
                .NotEmpty().WithMessage("Book name is required.")
                .MaximumLength(150);

            RuleFor(b => b.Pages)
                .InclusiveBetween(1, 5000);

            RuleFor(b => b.PublicationDate)
                .LessThanOrEqualTo(_ => DateOnly.FromDateTime(DateTime.Today.AddYears(1)))
                .WithMessage("Publication date can't be more than a year in the future.");
        }
    }
}
