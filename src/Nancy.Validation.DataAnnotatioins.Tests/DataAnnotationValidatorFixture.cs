//namespace Nancy.Validation.DataAnnotations.Tests
//{
//    using System.ComponentModel.DataAnnotations;
//    using System.Linq;
//    using Nancy.Tests;
//    using Nancy.Validation.DataAnnotations;
//    using Xunit;
//    using System.Collections.Generic;

//    public class DataAnnotationValidatorFixture
//    {
//        [Fact]
//        public void Should_not_throw_when_no_validation_attributes_exist()
//        {
//            // Given, When
//            var ex = Record.Exception(() => new DataAnnotationsValidator(typeof(string)));

//            // Then
//            ex.ShouldBeNull();
//        }

//        [Fact]
//        public void Should_invoke_validation()
//        {
//            // Given
//            var subject = new DataAnnotationsValidator(typeof(TestModel));
//            var instance = new TestModel { Age = "yeah" };

//            // When
//            var result = subject.Validate(instance);

//            // Then
//            result.IsValid.ShouldBeFalse();
//            result.Errors.ShouldHaveCount(3);
//        }

//        [Fact]
//        public void Description_should_be_correct()
//        {
//            // Given, When
//            var subject = new DataAnnotationsValidator(typeof(TestModel));

//            // Then
//            subject.Description.ShouldNotBeNull();
//            subject.Description.Rules.ShouldHaveCount(10);
//        }

//        [Fact]
//        public void Should_read_range_annotation()
//        {
//            // Given, When
//            var subject = new DataAnnotationsValidator(typeof(TestModel));

//            // Then
//            subject.Description.Rules.ShouldHave(r => r.RuleType == "Comparison" && r.MemberNames.Contains("Value"));
//            subject.Description.Rules.ShouldHave(r => r.RuleType == "Comparison" && r.MemberNames.Contains("Value"));
//        }

//        [Fact]
//        public void Should_read_regex_annotation()
//        {
//            // Given, When
//            var subject = new DataAnnotationsValidator(typeof(TestModel));

//            // Then
//            subject.Description.Rules.ShouldHave(r => r.RuleType == "Regex" && r.MemberNames.Contains("Age"));
//        }

//        [Fact]
//        public void Should_read_required_annotation()
//        {
//            // Given, When
//            var subject = new DataAnnotationsValidator(typeof(TestModel));

//            // Then
//            subject.Description.Rules.ShouldHave(r => r.RuleType == "NotNull" && r.MemberNames.Contains("FirstName"));
//            subject.Description.Rules.ShouldHave(r => r.RuleType == "NotEmpty" && r.MemberNames.Contains("FirstName"));
//        }

//        [Fact]
//        public void Should_read_string_length_annotation()
//        {
//            // Given, When
//            var subject = new DataAnnotationsValidator(typeof(TestModel));

//            // Then
//            subject.Description.Rules.ShouldHave(r => r.RuleType == "StringLength" && r.MemberNames.Contains("FirstName"));
//        }

//        [Fact]
//        public void Should_read_self_annotation()
//        {
//            // Given, When
//            var subject = new DataAnnotationsValidator(typeof(TestModel));

//            // Then
//            subject.Description.Rules.ShouldHave(r => r.RuleType == "Self" && r.MemberNames == null);
//        }

//        [Fact]
//        public void Should_use_custom_validator()
//        {
//            // Given
//            DataAnnotationsValidator.RegisterAdapter(typeof(OopsValidationAttribute), (a, d) => new OopsAdapter(a));

//            // When
//            var subject = new DataAnnotationsValidator(typeof(TestModel));

//            // Then
//            subject.Description.Rules.ShouldHave(r => r.RuleType == "Oops" && r.MemberNames == null);
//        }

//        [OopsValidation]
//        private class TestModel : IValidatableObject
//        {
//            [Required]
//            [StringLength(5)]
//            public string FirstName { get; set; }

//            [RegularExpression("\\d+")]
//            [Required]
//            public string Age { get; set; }

//            [Range(0, 10)]
//            public int Value { get; set; }

//            public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
//            {
//                return Enumerable.Empty<ValidationResult>();
//            }
//        }

//        private class OopsValidationAttribute : ValidationAttribute
//        {
//            protected override ValidationResult IsValid(object value, ValidationContext validationContext)
//            {
//                return new ValidationResult("Oops");
//            }
//        }

//        private class OopsAdapter : DataAnnotationsValidatorAdapter
//        {
//            public OopsAdapter(ValidationAttribute attribute)
//                : base("Oops", attribute)
//            {
//            }
//        }
//    }
//}