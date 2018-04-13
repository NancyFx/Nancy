namespace Nancy.Validation.DataAnnotations.Tests
{
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using Nancy.Tests;
    using Xunit;

    public class MaxLengthValidatorAdapterFixture : BaseValidatorAdapterTestFixture<MaxLengthValidatorAdapter>
    {
        [Fact]
        public void Should_return_true_with_valid_lengths()
        {
            // Given
            var validator = this.factory.Create(typeof(TestModel));
            var model = new TestModel
            {
                Password = "1",
                PasswordConfirmation = "12",
                PasswordConfirmationConfirmation = "123"
            };

            // When
            var result = validator.Validate(model, new NancyContext());

            // Then
            result.IsValid.ShouldBeTrue();
        }

        [Fact]
        public void Should_read_maxlength_annotations()
        {
            // Given, When
            var validator = this.factory.Create(typeof(TestModel));

            // Then
            validator.Description.Rules.SelectMany(r => r.Value).ShouldHave(r => r.RuleType == "MaxLength" && r.MemberNames.Contains("Password"));
            validator.Description.Rules.SelectMany(r => r.Value).ShouldHave(r => r.RuleType == "MaxLength" && r.MemberNames.Contains("PasswordConfirmation"));
            validator.Description.Rules.SelectMany(r => r.Value).ShouldHave(r => r.RuleType == "MaxLength" && r.MemberNames.Contains("PasswordConfirmationConfirmation"));
        }

        [Fact]
        public void Should_return_false_and_contain_property_name()
        {
            // Given
            var validator = this.factory.Create(typeof(TestModel));
            var model = new TestModel { Password = "1234" };

            // When
            var result = validator.Validate(model, new NancyContext());

            // Then
            result.IsValid.ShouldBeFalse();
            result.Errors["Password"][0].ErrorMessage.ShouldEqual("The field Password must be a string or array type with a maximum length of '3'.");
        }

        [Fact]
        public void Should_return_false_and_contain_display_name()
        {
            // Given
            var validator = this.factory.Create(typeof(TestModel));
            var model = new TestModel { PasswordConfirmation = "1234" };

            // When
            var result = validator.Validate(model, new NancyContext());

            // Then
            result.IsValid.ShouldBeFalse();
            result.Errors["PasswordConfirmation"][0].ErrorMessage.ShouldEqual("The field Password Confirmation must be a string or array type with a maximum length of '3'.");
        }

        [Fact]
        public void Should_return_false_and_contain_displayname()
        {
            // Given
            var validator = this.factory.Create(typeof(TestModel));
            var model = new TestModel { PasswordConfirmationConfirmation = "1234" };

            // When
            var result = validator.Validate(model, new NancyContext());

            // Then
            result.IsValid.ShouldBeFalse();
            result.Errors["PasswordConfirmationConfirmation"][0].ErrorMessage.ShouldEqual("The field Password Confirmation Confirmation must be a string or array type with a maximum length of '3'.");
        }

        private class TestModel
        {
            [MaxLength(3)]
            public string Password { get; set; }

            [Display(Name = "Password Confirmation")]
            [MaxLength(3)]
            public string PasswordConfirmation { get; set; }

            [DisplayName("Password Confirmation Confirmation")]
            [MaxLength(3)]
            public string PasswordConfirmationConfirmation { get; set; }
        }
    }
}
