//namespace Nancy.Validation.FluentValidation.Tests
//{
//    using System.Linq;
//    using Nancy.Tests;
//    using Rules;
//    using Xunit;
//    using global::FluentValidation;

//    public class FluentValidationValidatorFixture
//    {
//        [Fact]
//        public void Should_do_stuff()
//        {
//            // Given
//            var fluentValidator = new ClassUnderTestValidator();
//            var modelValidator = new FluentValidationValidator(fluentValidator);

//            // When
//            var result = modelValidator.Description;

//            // Then
//            result.Rules.Count().ShouldEqual(1);
//            result.Rules.First().ShouldBeOfType<StringLengthValidationRule>();
//            result.Rules.Cast<StringLengthValidationRule>().First().MaxLength.ShouldEqual(10);
//            result.Rules.Cast<StringLengthValidationRule>().First().MinLength.ShouldEqual(10);
//        }

//        [Fact]
//        public void Should_do_stuff2()
//        {
//            // Given
//            var fluentValidator = new ClassUnderTestValidator();
//            var modelValidator = new FluentValidationValidator(fluentValidator);

//            // When
//            var result = modelValidator.Description;

//            // Then
//            result.Rules.Count().ShouldEqual(1);
//            result.Rules.First().ShouldBeOfType<StringLengthValidationRule>();
//            result.Rules.Cast<StringLengthValidationRule>().First().MaxLength.ShouldEqual(10);
//            result.Rules.Cast<StringLengthValidationRule>().First().MinLength.ShouldEqual(1);
//        }

//        private class ClassUnderTest
//        {
//            //public string ExactLength { get; set; }

//            public string Length { get; set; }
//        }

//        private class ClassUnderTestValidator : AbstractValidator<ClassUnderTest>
//        {
//            public ClassUnderTestValidator()
//            {
//                //RuleFor(x => x.ExactLength).Length(10);

//                RuleFor(x => x.Length).Length(1, 10);

//            }
//        }
//    }
//}