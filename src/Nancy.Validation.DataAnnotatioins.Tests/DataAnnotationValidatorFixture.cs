namespace Nancy.Validation.DataAnnotations.Tests
{
    using System.ComponentModel.DataAnnotations;
    using System.Linq;

    using FakeItEasy;

    using Nancy.Tests;
    using Nancy.Validation.DataAnnotations;
    using Xunit;
    using System.Collections.Generic;

    public class DataAnnotationValidatorFixture
    {
        private readonly DataAnnotationsValidatorFactory factory;

        public DataAnnotationValidatorFixture()
        {
            var adapterFactory = new DefaultPropertyValidatorFactory(new IDataAnnotationsValidatorAdapter[]
            {
                new RangeValidatorAdapter(),
                new RegexValidatorAdapter(),
                new RequiredValidatorAdapter(),
                new StringLengthValidatorAdapter(),
                new OopsAdapter()
            });

            var validator = A.Fake<IValidatableObjectAdapter>();

            this.factory = new DataAnnotationsValidatorFactory(adapterFactory, validator);
        }

        [Fact]
        public void Should_not_throw_when_no_validation_attributes_exist()
        {
            // Given, When
            var ex = Record.Exception(() => this.factory.Create(typeof(string)));

            // Then
            ex.ShouldBeNull();
        }

        [Fact]
        public void Should_invoke_validation()
        {
            // Given
            var subject = this.factory.Create(typeof(TestModel));
            var instance = new TestModel { Age = "yeah" };

            // When
            var result = subject.Validate(instance, new NancyContext());

            // Then
            result.IsValid.ShouldBeFalse();
            result.Errors.ShouldHaveCount(3);
        }

        [Fact]
        public void Description_should_be_correct()
        {
            // Given, When
            var subject = this.factory.Create(typeof(TestModel));

            // Then
            subject.Description.ShouldNotBeNull();
            subject.Description.Rules.SelectMany(r => r.Value).ShouldHaveCount(9);
        }

        [Fact]
        public void Should_read_range_annotation()
        {
            // Given, When
            var subject = this.factory.Create(typeof(TestModel));

            // Then
            subject.Description.Rules.SelectMany(r => r.Value).ShouldHave(r => r.RuleType == "Comparison" && r.MemberNames.Contains("Value"));
            subject.Description.Rules.SelectMany(r => r.Value).ShouldHave(r => r.RuleType == "Comparison" && r.MemberNames.Contains("Value"));
        }

        [Fact]
        public void Should_read_regex_annotation()
        {
            // Given, When
            var subject = this.factory.Create(typeof(TestModel));

            // Then
            subject.Description.Rules.SelectMany(r => r.Value).ShouldHave(r => r.RuleType == "Regex" && r.MemberNames.Contains("Age"));
        }

        [Fact]
        public void Should_read_required_annotation()
        {
            // Given, When
            var subject = this.factory.Create(typeof(TestModel));

            // Then
            subject.Description.Rules.SelectMany(r => r.Value).ShouldHave(r => r.RuleType == "NotNull" && r.MemberNames.Contains("FirstName"));
            subject.Description.Rules.SelectMany(r => r.Value).ShouldHave(r => r.RuleType == "NotEmpty" && r.MemberNames.Contains("FirstName"));
        }

        [Fact]
        public void Should_read_string_length_annotation()
        {
            // Given, When
            var subject = this.factory.Create(typeof(TestModel));

            // Then
            subject.Description.Rules.SelectMany(r => r.Value).ShouldHave(r => r.RuleType == "StringLength" && r.MemberNames.Contains("FirstName"));
        }

        [Fact]
        public void Should_use_custom_validator()
        {
            // Given, When
            var subject = this.factory.Create(typeof(TestModel));

            // Then
            subject.Description.Rules.SelectMany(r => r.Value).ShouldHave(r => r.RuleType == "Oops" && r.MemberNames.Contains(string.Empty));
        }

        [OopsValidation]
        private class TestModel : IValidatableObject
        {
            [Required]
            [StringLength(5)]
            public string FirstName { get; set; }

            [RegularExpression("\\d+")]
            [Required]
            public string Age { get; set; }

            [Range(0, 10)]
            public int Value { get; set; }

            public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
            {
                return Enumerable.Empty<ValidationResult>();
            }
        }

        private class OopsValidationAttribute : ValidationAttribute
        {
            protected override ValidationResult IsValid(object value, ValidationContext validationContext)
            {
                return new ValidationResult("Oops", new[] { string.Empty });
            }
        }

        private class OopsAdapter : DataAnnotationsValidatorAdapter
        {
            public OopsAdapter()
                : base("Oops")
            {
            }

            public override bool CanHandle(ValidationAttribute attribute)
            {
                return attribute.GetType() == typeof(OopsValidationAttribute);
            }
        }
    }
}