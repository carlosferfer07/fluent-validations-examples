using FluentValidation;
using FluentValidation.TestHelper;
using FluentValidationExamples.Models;
using FluentValidationExamples.Validators;

namespace FluentValidationExamples.Tests.Validators
{
    public class UserRegistrationValidatorTests
    {
        private readonly UserRegistrationValidator _validator = new();

        private static UserRegistration ValidRegistration() => new()
        {
            Username = "maria_99",
            Email = "maria@mail.com",
            Password = "Str0ngPass",
            ConfirmPassword = "Str0ngPass",
            BirthDate = new DateOnly(1990, 5, 10),
            WantsNewsletter = false
        };

        [Fact]
        public async Task Valid_registration_has_no_errors()
        {
            var result = await _validator.TestValidateAsync(ValidRegistration());

            result.ShouldNotHaveAnyValidationErrors();
        }

        [Fact]
        public void Sync_validation_throws_because_of_the_async_username_rule()
        {
            Assert.Throws<AsyncValidatorInvokedSynchronouslyException>(() => _validator.Validate(ValidRegistration()));
        }

        [Theory]
        [InlineData("admin")]
        [InlineData("ROOT")]
        [InlineData("Carlos")]
        public async Task Taken_usernames_are_rejected_ignoring_case(string username)
        {
            var registration = ValidRegistration();
            registration.Username = username;

            var result = await _validator.TestValidateAsync(registration);

            result.ShouldHaveValidationErrorFor(u => u.Username)
                .WithErrorMessage($"Username '{username}' is already taken.");
        }

        [Theory]
        [InlineData("ab")]
        [InlineData("abcdefghijklmnopqrstu")]
        [InlineData("maria.99")]
        [InlineData("maria 99")]
        public async Task Username_with_wrong_length_or_characters_is_invalid(string username)
        {
            var registration = ValidRegistration();
            registration.Username = username;

            var result = await _validator.TestValidateAsync(registration);

            result.ShouldHaveValidationErrorFor(u => u.Username);
        }

        [Fact]
        public async Task Empty_username_only_reports_one_error()
        {
            var registration = ValidRegistration();
            registration.Username = "";

            var result = await _validator.TestValidateAsync(registration);

            // Cascade stop means the length, regex and availability checks don't pile up
            Assert.Single(result.Errors, e => e.PropertyName == nameof(UserRegistration.Username));
        }

        [Theory]
        [InlineData("")]
        [InlineData("not-an-email")]
        public async Task Email_must_be_valid(string email)
        {
            var registration = ValidRegistration();
            registration.Email = email;

            var result = await _validator.TestValidateAsync(registration);

            result.ShouldHaveValidationErrorFor(u => u.Email);
        }

        [Fact]
        public async Task Password_shorter_than_8_characters_is_invalid()
        {
            var registration = ValidRegistration();
            registration.Password = registration.ConfirmPassword = "Ab1defg";

            var result = await _validator.TestValidateAsync(registration);

            result.ShouldHaveValidationErrorFor(u => u.Password);
        }

        [Theory]
        [InlineData("str0ngpass", "Password must contain at least one uppercase letter.")]
        [InlineData("STR0NGPASS", "Password must contain at least one lowercase letter.")]
        [InlineData("StrongPass", "Password must contain at least one number.")]
        public async Task Password_must_mix_upper_lower_and_digits(string password, string expectedMessage)
        {
            var registration = ValidRegistration();
            registration.Password = registration.ConfirmPassword = password;

            var result = await _validator.TestValidateAsync(registration);

            result.ShouldHaveValidationErrorFor(u => u.Password).WithErrorMessage(expectedMessage);
        }

        [Fact]
        public async Task Confirm_password_must_match()
        {
            var registration = ValidRegistration();
            registration.ConfirmPassword = "Different1";

            var result = await _validator.TestValidateAsync(registration);

            result.ShouldHaveValidationErrorFor(u => u.ConfirmPassword)
                .WithErrorMessage("Passwords don't match.");
        }

        [Fact]
        public async Task User_turning_18_today_can_register()
        {
            var registration = ValidRegistration();
            registration.BirthDate = DateOnly.FromDateTime(DateTime.Today.AddYears(-18));

            var result = await _validator.TestValidateAsync(registration);

            result.ShouldNotHaveValidationErrorFor(u => u.BirthDate);
        }

        [Fact]
        public async Task User_turning_18_tomorrow_cannot_register()
        {
            var registration = ValidRegistration();
            registration.BirthDate = DateOnly.FromDateTime(DateTime.Today.AddYears(-18).AddDays(1));

            var result = await _validator.TestValidateAsync(registration);

            result.ShouldHaveValidationErrorFor(u => u.BirthDate)
                .WithErrorMessage("You must be at least 18 years old to register.");
        }

        [Fact]
        public async Task Newsletter_frequency_is_ignored_when_not_subscribing()
        {
            var registration = ValidRegistration();
            registration.WantsNewsletter = false;
            registration.NewsletterFrequency = "every second";

            var result = await _validator.TestValidateAsync(registration);

            result.ShouldNotHaveValidationErrorFor(u => u.NewsletterFrequency);
        }

        [Fact]
        public async Task Missing_newsletter_frequency_is_an_error_not_a_crash()
        {
            var registration = ValidRegistration();
            registration.WantsNewsletter = true;
            registration.NewsletterFrequency = null;

            var result = await _validator.TestValidateAsync(registration);

            Assert.Single(result.Errors, e => e.PropertyName == nameof(UserRegistration.NewsletterFrequency));
        }

        [Theory]
        [InlineData("daily")]
        [InlineData("Weekly")]
        [InlineData("MONTHLY")]
        public async Task Known_newsletter_frequencies_are_valid(string frequency)
        {
            var registration = ValidRegistration();
            registration.WantsNewsletter = true;
            registration.NewsletterFrequency = frequency;

            var result = await _validator.TestValidateAsync(registration);

            result.ShouldNotHaveValidationErrorFor(u => u.NewsletterFrequency);
        }

        [Fact]
        public async Task Unknown_newsletter_frequency_is_invalid()
        {
            var registration = ValidRegistration();
            registration.WantsNewsletter = true;
            registration.NewsletterFrequency = "yearly";

            var result = await _validator.TestValidateAsync(registration);

            result.ShouldHaveValidationErrorFor(u => u.NewsletterFrequency)
                .WithErrorMessage("Frequency must be one of: daily, weekly, monthly.");
        }
    }
}
