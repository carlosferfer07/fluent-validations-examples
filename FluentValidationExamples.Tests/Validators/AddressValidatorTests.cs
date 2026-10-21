using FluentValidation.TestHelper;
using FluentValidationExamples.Models;
using FluentValidationExamples.Validators;

namespace FluentValidationExamples.Tests.Validators
{
    public class AddressValidatorTests
    {
        private readonly AddressValidator _validator = new();

        private static Address ValidAddress() => new()
        {
            Street = "Rua Augusta 10",
            City = "Lisboa",
            PostalCode = "1100-053",
            Country = "PT"
        };

        [Fact]
        public void Valid_address_has_no_errors()
        {
            _validator.TestValidate(ValidAddress()).ShouldNotHaveAnyValidationErrors();
        }

        [Fact]
        public void Street_and_city_are_required()
        {
            var address = ValidAddress();
            address.Street = "";
            address.City = null;

            var result = _validator.TestValidate(address);

            result.ShouldHaveValidationErrorFor(a => a.Street);
            result.ShouldHaveValidationErrorFor(a => a.City);
        }

        [Fact]
        public void Street_and_city_have_max_lengths()
        {
            var address = ValidAddress();
            address.Street = new string('a', 201);
            address.City = new string('a', 101);

            var result = _validator.TestValidate(address);

            result.ShouldHaveValidationErrorFor(a => a.Street);
            result.ShouldHaveValidationErrorFor(a => a.City);
        }

        [Theory]
        [InlineData("")]
        [InlineData("P")]
        [InlineData("PRT")]
        public void Country_must_be_a_two_letter_code(string country)
        {
            var address = ValidAddress();
            address.Country = country;

            _validator.TestValidate(address).ShouldHaveValidationErrorFor(a => a.Country);
        }

        [Theory]
        [InlineData("1100")]
        [InlineData("1100053")]
        [InlineData("11-00053")]
        public void Portuguese_postal_code_must_match_format(string postalCode)
        {
            var address = ValidAddress();
            address.PostalCode = postalCode;

            _validator.TestValidate(address)
                .ShouldHaveValidationErrorFor(a => a.PostalCode)
                .WithErrorMessage("Portuguese postal codes must look like 1234-567.");
        }

        [Fact]
        public void Other_countries_skip_the_portuguese_format()
        {
            var address = ValidAddress();
            address.Country = "US";
            address.PostalCode = "90210";

            _validator.TestValidate(address).ShouldNotHaveValidationErrorFor(a => a.PostalCode);
        }

        [Theory]
        [InlineData("PT")]
        [InlineData("US")]
        public void Postal_code_is_required_for_every_country(string country)
        {
            var address = ValidAddress();
            address.Country = country;
            address.PostalCode = "";

            _validator.TestValidate(address).ShouldHaveValidationErrorFor(a => a.PostalCode);
        }
    }
}
