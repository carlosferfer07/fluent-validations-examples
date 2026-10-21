using FluentValidation.TestHelper;
using FluentValidationExamples.Models;
using FluentValidationExamples.Validators;

namespace FluentValidationExamples.Tests.Validators
{
    public class BookValidatorTests
    {
        private readonly BookValidator _validator = new();

        private static Book ValidBook() => new()
        {
            Name = "Dune",
            Pages = 412,
            PublicationDate = new DateOnly(1965, 8, 1)
        };

        [Fact]
        public void Valid_book_has_no_errors()
        {
            _validator.TestValidate(ValidBook()).ShouldNotHaveAnyValidationErrors();
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void Name_is_required(string? name)
        {
            var book = ValidBook();
            book.Name = name;

            _validator.TestValidate(book)
                .ShouldHaveValidationErrorFor(b => b.Name)
                .WithErrorMessage("Book name is required.");
        }

        [Fact]
        public void Name_can_have_150_characters()
        {
            var book = ValidBook();
            book.Name = new string('a', 150);

            _validator.TestValidate(book).ShouldNotHaveValidationErrorFor(b => b.Name);
        }

        [Fact]
        public void Name_longer_than_150_characters_is_invalid()
        {
            var book = ValidBook();
            book.Name = new string('a', 151);

            _validator.TestValidate(book).ShouldHaveValidationErrorFor(b => b.Name);
        }

        [Theory]
        [InlineData(1)]
        [InlineData(5000)]
        public void Pages_within_range_are_valid(int pages)
        {
            var book = ValidBook();
            book.Pages = pages;

            _validator.TestValidate(book).ShouldNotHaveValidationErrorFor(b => b.Pages);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        [InlineData(5001)]
        public void Pages_outside_range_are_invalid(int pages)
        {
            var book = ValidBook();
            book.Pages = pages;

            _validator.TestValidate(book).ShouldHaveValidationErrorFor(b => b.Pages);
        }

        [Fact]
        public void Publication_date_exactly_one_year_ahead_is_valid()
        {
            var book = ValidBook();
            book.PublicationDate = DateOnly.FromDateTime(DateTime.Today.AddYears(1));

            _validator.TestValidate(book).ShouldNotHaveValidationErrorFor(b => b.PublicationDate);
        }

        [Fact]
        public void Publication_date_more_than_one_year_ahead_is_invalid()
        {
            var book = ValidBook();
            book.PublicationDate = DateOnly.FromDateTime(DateTime.Today.AddYears(1).AddDays(1));

            _validator.TestValidate(book)
                .ShouldHaveValidationErrorFor(b => b.PublicationDate)
                .WithErrorMessage("Publication date can't be more than a year in the future.");
        }
    }
}
