using System.Globalization;
using FluentValidation.TestHelper;
using FluentValidationExamples.Models;
using FluentValidationExamples.Validators;

namespace FluentValidationExamples.Tests.Validators
{
    public class OrderItemValidatorTests
    {
        private readonly OrderItemValidator _validator = new();

        private static OrderItem ValidItem() => new()
        {
            ProductCode = "ABC-1234",
            Quantity = 2,
            UnitPrice = 9.99m
        };

        [Fact]
        public void Valid_item_has_no_errors()
        {
            _validator.TestValidate(ValidItem()).ShouldNotHaveAnyValidationErrors();
        }

        [Theory]
        [InlineData("")]
        [InlineData("abc-1234")]
        [InlineData("AB-1234")]
        [InlineData("ABC-123")]
        [InlineData("ABC1234")]
        public void Product_code_must_look_like_ABC_1234(string code)
        {
            var item = ValidItem();
            item.ProductCode = code;

            _validator.TestValidate(item).ShouldHaveValidationErrorFor(i => i.ProductCode);
        }

        [Theory]
        [InlineData(1)]
        [InlineData(100)]
        public void Quantity_within_range_is_valid(int quantity)
        {
            var item = ValidItem();
            item.Quantity = quantity;

            _validator.TestValidate(item).ShouldNotHaveValidationErrorFor(i => i.Quantity);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(101)]
        public void Quantity_outside_range_is_invalid(int quantity)
        {
            var item = ValidItem();
            item.Quantity = quantity;

            _validator.TestValidate(item).ShouldHaveValidationErrorFor(i => i.Quantity);
        }

        [Theory]
        [InlineData("0")]
        [InlineData("-1")]
        [InlineData("9.999")]
        public void Unit_price_must_be_positive_with_at_most_two_decimals(string price)
        {
            var item = ValidItem();
            item.UnitPrice = decimal.Parse(price, CultureInfo.InvariantCulture);

            _validator.TestValidate(item).ShouldHaveValidationErrorFor(i => i.UnitPrice);
        }
    }
}
