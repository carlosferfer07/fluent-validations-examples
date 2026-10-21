using FluentValidation.TestHelper;
using FluentValidationExamples.Models;
using FluentValidationExamples.Validators;

namespace FluentValidationExamples.Tests.Validators
{
    public class OrderValidatorTests
    {
        private readonly OrderValidator _validator = new();

        private static Order ValidOrder() => new()
        {
            CustomerEmail = "john@mail.com",
            ShippingAddress = new Address
            {
                Street = "Rua Augusta 10",
                City = "Lisboa",
                PostalCode = "1100-053",
                Country = "PT"
            },
            Items =
            {
                new OrderItem { ProductCode = "ABC-1234", Quantity = 2, UnitPrice = 10m },
                new OrderItem { ProductCode = "XYZ-9876", Quantity = 1, UnitPrice = 5m }
            }
        };

        [Fact]
        public void Valid_order_has_no_errors()
        {
            _validator.TestValidate(ValidOrder()).ShouldNotHaveAnyValidationErrors();
        }

        [Fact]
        public void Customer_email_must_be_valid()
        {
            var order = ValidOrder();
            order.CustomerEmail = "john";

            _validator.TestValidate(order).ShouldHaveValidationErrorFor(o => o.CustomerEmail);
        }

        [Fact]
        public void Shipping_address_is_required()
        {
            var order = ValidOrder();
            order.ShippingAddress = null;

            _validator.TestValidate(order).ShouldHaveValidationErrorFor(o => o.ShippingAddress);
        }

        [Fact]
        public void Shipping_address_errors_come_from_the_address_validator()
        {
            var order = ValidOrder();
            order.ShippingAddress!.PostalCode = "1100";

            _validator.TestValidate(order)
                .ShouldHaveValidationErrorFor("ShippingAddress.PostalCode")
                .WithErrorMessage("Portuguese postal codes must look like 1234-567.");
        }

        [Fact]
        public void Order_without_items_is_invalid()
        {
            var order = ValidOrder();
            order.Items.Clear();

            _validator.TestValidate(order)
                .ShouldHaveValidationErrorFor(o => o.Items)
                .WithErrorMessage("An order needs at least one item.");
        }

        [Fact]
        public void Each_item_is_validated_with_its_index()
        {
            var order = ValidOrder();
            order.Items[1].Quantity = 0;

            var result = _validator.TestValidate(order);

            result.ShouldHaveValidationErrorFor("Items[1].Quantity");
            result.ShouldNotHaveValidationErrorFor("Items[0].Quantity");
        }

        [Fact]
        public void Same_product_twice_is_invalid()
        {
            var order = ValidOrder();
            order.Items[1].ProductCode = order.Items[0].ProductCode;

            _validator.TestValidate(order)
                .ShouldHaveValidationErrorFor(o => o.Items)
                .WithErrorMessage("The same product shouldn't appear twice, update the quantity instead.");
        }

        [Fact]
        public void No_discount_code_skips_discount_rules_even_for_small_orders()
        {
            var order = ValidOrder();
            order.Items.RemoveAt(0); // total is now 5
            order.DiscountCode = null;

            var result = _validator.TestValidate(order);

            result.ShouldNotHaveValidationErrorFor(o => o.DiscountCode);
            result.ShouldNotHaveValidationErrorFor(o => o.Total);
        }

        [Fact]
        public void Discount_code_must_start_with_PROMO()
        {
            var order = ValidOrder();
            order.DiscountCode = "SALE10";

            _validator.TestValidate(order)
                .ShouldHaveValidationErrorFor(o => o.DiscountCode)
                .WithErrorMessage("Invalid discount code.");
        }

        [Fact]
        public void Discount_code_works_when_total_is_exactly_20()
        {
            var order = ValidOrder();
            order.Items.RemoveAt(1); // 2 x 10 = 20
            order.DiscountCode = "PROMO10";

            _validator.TestValidate(order).ShouldNotHaveAnyValidationErrors();
        }

        [Fact]
        public void Discount_code_on_order_below_20_is_invalid()
        {
            var order = ValidOrder();
            order.Items[0].Quantity = 1; // 10 + 5 = 15
            order.DiscountCode = "PROMO10";

            _validator.TestValidate(order)
                .ShouldHaveValidationErrorFor(o => o.Total)
                .WithErrorMessage("Discount codes only work on orders of 20 or more (current total: 15).");
        }
    }
}
