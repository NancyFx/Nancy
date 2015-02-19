namespace Nancy.Validation.FluentValidation.Tests
{
    using FakeItEasy;

    using global::FluentValidation;

    using Nancy.Tests;

    using Xunit;

    public class FluentValidationValidatorFactoryFixture
    {
        private readonly FluentValidationValidatorFactory factory;

        public FluentValidationValidatorFactoryFixture()
        {
            var adapterFactory = 
                A.Fake<IFluentAdapterFactory>();

            var validators = 
                new IValidator[] {new ClassUnderTestValidator(), new NeverBeUsedTestValidator()};

            this.factory = 
                new FluentValidationValidatorFactory(adapterFactory, validators);
        }

        [Fact]
        public void Should_return_instance_when_validator_was_found_for_type()
        {
            // Given
            // When
            var instance = this.factory.Create(typeof(ClassUnderTest));

            // Then
            instance.ShouldNotBeNull();
        }

        [Fact]
        public void Should_return_null_when_no_validator_was_found_for_type()
        {
            // Given
            // When
            var instance = this.factory.Create(typeof(string));

            // Then
            instance.ShouldBeNull();
        }

        public class ClassUnderTest
        {
        }

        public class ClassUnderTestValidator : AbstractValidator<ClassUnderTest>
        {
        }

        public class NeverBeUsedTestValidator : AbstractValidator<int>
        {
        }
    }
}