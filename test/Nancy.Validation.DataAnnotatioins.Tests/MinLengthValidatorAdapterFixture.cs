namespace Nancy.Validation.DataAnnotations.Tests
{
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using Nancy.Tests;
    using Xunit;

    public class MinLengthValidatorAdapterFixture : BaseValidatorAdapterTestFixture<MinLengthValidatorAdapter>
    {
        [Fact]
        public void Should_return_true_with_valid_lengths()
        {
            // Given
            var validator = this.factory.Create(typeof(TestModel));
            var model = new TestModel
            {
                Password = "12",
                PasswordConfirmation = "123",
                PasswordConfirmationConfirmation = "1234"
            };

            // When
            var result = validator.Validate(model, new NancyContext());

            // Then
            result.IsValid.ShouldBeTrue();
        }

        [Fact]
        public void Should_read_minlength_annotations()
        {
            // Given, When
            var validator = this.factory.Create(typeof(TestModel));

            // Then
            validator.Description.Rules.SelectMany(r => r.Value).ShouldHave(r => r.RuleType == "MinLength" && r.MemberNames.Contains("Password"));
            validator.Description.Rules.SelectMany(r => r.Value).ShouldHave(r => r.RuleType == "MinLength" && r.MemberNames.Contains("PasswordConfirmation"));
            validator.Description.Rules.SelectMany(r => r.Value).ShouldHave(r => r.RuleType == "MinLength" && r.MemberNames.Contains("PasswordConfirmationConfirmation"));
        }

        [Fact]
        public void Should_return_false_and_contain_property_name()
        {
            // Given
            var validator = this.factory.Create(typeof(TestModel));
            var model = new TestModel { Password = "1" };

            // When
            var result = validator.Validate(model, new NancyContext());

            // Then
            result.IsValid.ShouldBeFalse();
            result.Errors["Password"][0].ErrorMessage.ShouldEqual("The field Password must be a string or array type with a minimum length of '2'.");
        }

        [Fact]
        public void Should_return_false_and_contain_display_name()
        {
            // Given
            var validator = this.factory.Create(typeof(TestModel));
            var model = new TestModel { PasswordConfirmation = "1" };

            // When
            var result = validator.Validate(model, new NancyContext());

            // Then
            result.IsValid.ShouldBeFalse();
            result.Errors["PasswordConfirmation"][0].ErrorMessage.ShouldEqual("The field Password Confirmation must be a string or array type with a minimum length of '2'.");
        }

        [Fact]
        public void Should_return_false_and_contain_displayname()
        {
            // Given
            var validator = this.factory.Create(typeof(TestModel));
            var model = new TestModel { PasswordConfirmationConfirmation = "1" };

            // When
            var result = validator.Validate(model, new NancyContext());

            // Then
            result.IsValid.ShouldBeFalse();
            result.Errors["PasswordConfirmationConfirmation"][0].ErrorMessage.ShouldEqual("The field Password Confirmation Confirmation must be a string or array type with a minimum length of '2'.");
        }

        private class TestModel
        {
            [MinLength(2)]
            public string Password { get; set; }

            [Display(Name = "Password Confirmation")]
            [MinLength(2)]
            public string PasswordConfirmation { get; set; }

            [DisplayName("Password Confirmation Confirmation")]
            [MinLength(2)]
            public string PasswordConfirmationConfirmation { get; set; }
        }
    }
}
