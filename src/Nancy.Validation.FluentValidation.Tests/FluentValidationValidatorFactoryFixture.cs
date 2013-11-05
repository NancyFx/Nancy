//namespace Nancy.Validation.FluentValidation.Tests
//{
//    using FakeItEasy;
//    using Nancy.Tests;
//    using Xunit;
//    using global::FluentValidation;

//    public class FluentValidationValidatorFactoryFixture
//    {
//        [Fact]
//        public void Should_return_instance_when_validator_was_found_for_type()
//        {
//            // Given
//            var factory = new FluentValidationValidatorFactory(A.Fake<IFluentAdapterFactory>());

//            // When
//            var instance = factory.Create(typeof(ClassUnderTest));

//            // Then
//            instance.ShouldNotBeNull();
//        }

//        [Fact]
//        public void Should_return_null_when_no_validator_was_found_for_type()
//        {
//            // Given
//            var factory = new FluentValidationValidatorFactory(A.Fake<IFluentAdapterFactory>());

//            // When
//            var instance = factory.Create(typeof(string));

//            // Then
//            instance.ShouldBeNull();
//        }

//        public class ClassUnderTest
//        {
//        }

//        public class ClassUnderTestValidator : AbstractValidator<ClassUnderTest>
//        {
//        }
//    }
//}