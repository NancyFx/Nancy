namespace Nancy.Tests.Unit.Validation.DataAnnotations
{
    using System;
    using DA = System.ComponentModel.DataAnnotations;
    using System.Linq;
    using FakeItEasy;
    using Nancy.Validation;
    using Nancy.Validation.DataAnnotations;
    using Xunit;
    using System.Collections.Generic;

    public class DataAnnotationValidatorFixture
    {
        [Fact]
        public void Should_not_throw_when_no_validation_attributes_exist()
        {
            var ex = Record.Exception(() => new DataAnnotationsValidator(typeof(string)));

            ex.ShouldBeNull();
        }

        [Fact]
        public void Should_invoke_validation()
        {
            var subject = new DataAnnotationsValidator(typeof(TestModel));
            var instance = new TestModel { Age = "yeah" };

            var result = subject.Validate(instance);

            result.IsValid.ShouldBeFalse();
            result.Errors.ShouldHaveCount(3);
        }

        [Fact]
        public void Description_should_be_correct()
        {
            var subject = new DataAnnotationsValidator(typeof(TestModel));

            subject.Description.ShouldNotBeNull();
            subject.Description.Rules.ShouldHaveCount(10);
        }

        [Fact]
        public void Should_read_range_annotation()
        {
            var subject = new DataAnnotationsValidator(typeof(TestModel));

            subject.Description.Rules.ShouldHave(r => r.RuleType == "Comparison" && r.MemberNames.Contains("Value"));
            subject.Description.Rules.ShouldHave(r => r.RuleType == "Comparison" && r.MemberNames.Contains("Value"));
        }

        [Fact]
        public void Should_read_regex_annotation()
        {
            var subject = new DataAnnotationsValidator(typeof(TestModel));

            subject.Description.Rules.ShouldHave(r => r.RuleType == "Regex" && r.MemberNames.Contains("Age"));
        }

        [Fact]
        public void Should_read_required_annotation()
        {
            var subject = new DataAnnotationsValidator(typeof(TestModel));

            subject.Description.Rules.ShouldHave(r => r.RuleType == "NotNull" && r.MemberNames.Contains("FirstName"));
            subject.Description.Rules.ShouldHave(r => r.RuleType == "NotEmpty" && r.MemberNames.Contains("FirstName"));
        }

        [Fact]
        public void Should_read_string_length_annotation()
        {
            var subject = new DataAnnotationsValidator(typeof(TestModel));

            subject.Description.Rules.ShouldHave(r => r.RuleType == "StringLength" && r.MemberNames.Contains("FirstName"));
        }

        [Fact]
        public void Should_read_self_annotation()
        {
            var subject = new DataAnnotationsValidator(typeof(TestModel));

            subject.Description.Rules.ShouldHave(r => r.RuleType == "Self" && r.MemberNames == null);
        }

        [Fact]
        public void Should_use_custom_validator()
        {
            DataAnnotationsValidator.RegisterAdapter(typeof(OopsValidationAttribute), (a, d) => new OopsAdapter(a));

            var subject = new DataAnnotationsValidator(typeof(TestModel));
            subject.Description.Rules.ShouldHave(r => r.RuleType == "Oops" && r.MemberNames == null);
        }

        [OopsValidation]
        private class TestModel : DA.IValidatableObject
        {
            [DA.Required]
            [DA.StringLength(5)]
            public string FirstName { get; set; }

            [DA.RegularExpression("\\d+")]
            [DA.Required]
            public string Age { get; set; }

            [DA.Range(0, 10)]
            public int Value { get; set; }

            public IEnumerable<DA.ValidationResult> Validate(DA.ValidationContext validationContext)
            {
                return Enumerable.Empty<DA.ValidationResult>();
            }
        }

        private class OopsValidationAttribute : DA.ValidationAttribute
        {
            protected override DA.ValidationResult IsValid(object value, DA.ValidationContext validationContext)
            {
                return new DA.ValidationResult("Oops");
            }
        }

        private class OopsAdapter : DataAnnotationsValidatorAdapter
        {
            public OopsAdapter(DA.ValidationAttribute attribute)
                : base("Oops", attribute)
            { }
        }
    }
}