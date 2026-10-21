using FluentValidation;
using FluentValidationExamples.Models;

namespace FluentValidationExamples.Validators
{
    public class UserRegistrationValidator : AbstractValidator<UserRegistration>
    {
        // Pretend these already exist in a database
        private static readonly string[] TakenUsernames = { "admin", "root", "carlos" };
        private static readonly string[] Frequencies = { "daily", "weekly", "monthly" };

        public UserRegistrationValidator()
        {
            RuleFor(u => u.Username)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .Length(3, 20)
                .Matches("^[a-zA-Z0-9_]+$").WithMessage("Username can only contain letters, numbers and underscores.")
                .MustAsync(BeAvailableAsync).WithMessage("Username '{PropertyValue}' is already taken.");

            RuleFor(u => u.Email)
                .NotEmpty()
                .EmailAddress();

            RuleFor(u => u.Password)
                .NotEmpty()
                .MinimumLength(8)
                .Matches("[A-Z]").WithMessage("Password must contain at least one uppercase letter.")
                .Matches("[a-z]").WithMessage("Password must contain at least one lowercase letter.")
                .Matches("[0-9]").WithMessage("Password must contain at least one number.");

            RuleFor(u => u.ConfirmPassword)
                .Equal(u => u.Password).WithMessage("Passwords don't match.");

            RuleFor(u => u.BirthDate)
                .Must(BeAtLeast18).WithMessage("You must be at least 18 years old to register.");

            When(u => u.WantsNewsletter, () =>
            {
                RuleFor(u => u.NewsletterFrequency)
                    .NotEmpty()
                    .Must(f => Frequencies.Contains(f!.ToLower()))
                    .WithMessage("Frequency must be one of: daily, weekly, monthly.");
            });
        }

        private static async Task<bool> BeAvailableAsync(string? username, CancellationToken cancellationToken)
        {
            // Simulates a database lookup
            await Task.Delay(10, cancellationToken);
            return !TakenUsernames.Contains(username, StringComparer.OrdinalIgnoreCase);
        }

        private static bool BeAtLeast18(DateOnly birthDate)
        {
            return birthDate <= DateOnly.FromDateTime(DateTime.Today.AddYears(-18));
        }
    }
}
