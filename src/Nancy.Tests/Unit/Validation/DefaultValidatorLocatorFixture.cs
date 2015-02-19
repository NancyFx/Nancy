namespace Nancy.Tests.Unit.Validation
{
    using System;

    using FakeItEasy;

    using Nancy.Validation;

    using Xunit;

    public class DefaultValidatorLocatorFixture
    {
        [Fact]
        public void Should_not_throw_if_null_validator_locators_collection_is_passed()
        {
            // Given, When
            var result = Record.Exception(() => new DefaultValidatorLocator(null));

            // Then
            result.ShouldBeNull();
        }

        [Fact]
        public void Should_return_null_if_no_validator_can_validate_the_given_type()
        {
            // Given
            var fakeFactories = A.CollectionOfFake<IModelValidatorFactory>(3);
            A.CallTo(() => fakeFactories[0].Create(A<Type>.Ignored)).Returns(null);
            A.CallTo(() => fakeFactories[1].Create(A<Type>.Ignored)).Returns(null);
            A.CallTo(() => fakeFactories[2].Create(A<Type>.Ignored)).Returns(null);
            var subject = new DefaultValidatorLocator(fakeFactories);

            // When
            var result = subject.GetValidatorForType(typeof(string));

            // Then
            result.ShouldBeNull();
        }

        [Fact]
        public void Should_return_the_original_validator_when_only_one_factory_can_provide_a_validator_for_the_given_type()
        {
            // Given
            var fakeFactories = A.CollectionOfFake<IModelValidatorFactory>(3);
            var fakeValidator = A.Fake<IModelValidator>();
            A.CallTo(() => fakeFactories[0].Create(A<Type>.Ignored)).Returns(null);
            A.CallTo(() => fakeFactories[1].Create(A<Type>.Ignored)).Returns(fakeValidator);
            A.CallTo(() => fakeFactories[2].Create(A<Type>.Ignored)).Returns(null);

            var subject = new DefaultValidatorLocator(fakeFactories);

            // When
            var result = subject.GetValidatorForType(typeof(string));

            // Then
            result.ShouldBeSameAs(fakeValidator);
        }

        [Fact]
        public void Should_return_a_composite_validator_when_more_than_one_factory_can_provide_a_validator_for_the_given_type()
        {
            // Given
            var fakeFactories = A.CollectionOfFake<IModelValidatorFactory>(3);
            var fakeValidator1 = A.Fake<IModelValidator>();
            var fakeValidator2 = A.Fake<IModelValidator>();
            A.CallTo(() => fakeFactories[1].Create(A<Type>.Ignored)).Returns(fakeValidator1);
            A.CallTo(() => fakeFactories[2].Create(A<Type>.Ignored)).Returns(fakeValidator2);

            var subject = new DefaultValidatorLocator(fakeFactories);

            // When
            var result = subject.GetValidatorForType(typeof(string));

            // Then
            result.ShouldBeOfType<CompositeValidator>();
        }

        [Fact]
        public void Should_throw_modelvalidationexception_when_retrieving_validator_but_no_factories_have_been_registered()
        {
            // Given
            var subject = new DefaultValidatorLocator(null);

            // When
            var exception = Record.Exception(() => subject.GetValidatorForType(typeof(string)));

            // Then
            exception.ShouldBeOfType<ModelValidationException>();
        }
    }
}