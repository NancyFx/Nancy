namespace Nancy.Validation.DataAnnotations.Tests
{
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using Nancy.Tests;
    using Xunit;

    public class CreditCardValidatorAdapterFixture : BaseValidatorAdapterTestFixture<CreditCardValidatorAdapter>
    {
        [Fact]
        public void Should_return_true_with_valid_card_numbers()
        {
            // Given
            var validator = this.factory.Create(typeof(TestModel));
            var model = new TestModel
            {
                MasterCard = "5555 5555 5555 4444",
                VisaCard = "4242-4242-4242-4242",
                DiscoverCard = "6011-1111-1111-1117"
            };

            // When
            var result = validator.Validate(model, new NancyContext());

            // Then
            result.IsValid.ShouldBeTrue();
        }

        [Fact]
        public void Should_read_creditcard_annotations()
        {
            // Given, When
            var validator = this.factory.Create(typeof(TestModel));

            // Then
            validator.Description.Rules.SelectMany(r => r.Value).ShouldHave(r => r.RuleType == "CreditCard" && r.MemberNames.Contains("MasterCard"));
            validator.Description.Rules.SelectMany(r => r.Value).ShouldHave(r => r.RuleType == "CreditCard" && r.MemberNames.Contains("VisaCard"));
            validator.Description.Rules.SelectMany(r => r.Value).ShouldHave(r => r.RuleType == "CreditCard" && r.MemberNames.Contains("DiscoverCard"));
        }

#if !__MonoCS__

        [Fact]
        public void Should_return_false_and_contain_property_name()
        {
            // Given
            var validator = this.factory.Create(typeof(TestModel));
            var model = new TestModel { MasterCard = "mastercard" };

            // When
            var result = validator.Validate(model, new NancyContext());

            // Then
            result.IsValid.ShouldBeFalse();
            result.Errors["MasterCard"][0].ErrorMessage.ShouldEqual("The MasterCard field is not a valid credit card number.");
        }

        [Fact]
        public void Should_return_false_and_contain_display_name()
        {
            // Given
            var validator = this.factory.Create(typeof(TestModel));
            var model = new TestModel { VisaCard = "visa" };

            // When
            var result = validator.Validate(model, new NancyContext());

            // Then
            result.IsValid.ShouldBeFalse();
            result.Errors["VisaCard"][0].ErrorMessage.ShouldEqual("The Visa Card field is not a valid credit card number.");
        }

        [Fact]
        public void Should_return_false_and_contain_displayname()
        {
            // Given
            var validator = this.factory.Create(typeof(TestModel));
            var model = new TestModel { DiscoverCard = "discover" };

            // When
            var result = validator.Validate(model, new NancyContext());

            // Then
            result.IsValid.ShouldBeFalse();
            result.Errors["DiscoverCard"][0].ErrorMessage.ShouldEqual("The Discover Card field is not a valid credit card number.");
        }

#endif

        private class TestModel
        {
            [CreditCard]
            public string MasterCard { get; set; }

            [Display(Name = "Visa Card")]
            [CreditCard]
            public string VisaCard { get; set; }

            [DisplayName("Discover Card")]
            [CreditCard]
            public string DiscoverCard { get; set; }
        }
    }
}
