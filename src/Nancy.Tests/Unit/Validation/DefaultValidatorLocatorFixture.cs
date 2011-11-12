namespace Nancy.Tests.Unit.Validation
{
    using System;
    using FakeItEasy;
    using Nancy.Validation;
    using Xunit;

    public class DefaultValidatorLocatorFixture
    {
        [Fact]
        public void Should_not_throw_if_null_validator_locatorss_collection_is_passed()
        {
            var result = Record.Exception(() => new DefaultValidatorLocator(null));

            result.ShouldBeNull();
        }

        [Fact]
        public void Should_return_null_if_no_validator_can_validate_the_given_type()
        {
            var fakeFactories = A.CollectionOfFake<IValidatorFactory>(3);
            A.CallTo(() => fakeFactories[0].Create(A<Type>.Ignored)).Returns(null);
            A.CallTo(() => fakeFactories[1].Create(A<Type>.Ignored)).Returns(null);
            A.CallTo(() => fakeFactories[2].Create(A<Type>.Ignored)).Returns(null);
            var subject = new DefaultValidatorLocator(fakeFactories);

            var result = subject.GetValidatorForType(typeof(string));

            result.ShouldBeNull();
        }

        [Fact]
        public void Should_return_the_original_validator_when_only_one_factory_can_provide_a_validator_for_the_given_type()
        {
            var fakeFactories = A.CollectionOfFake<IValidatorFactory>(3);
            var fakeValidator = A.Fake<IValidator>();
            A.CallTo(() => fakeFactories[0].Create(A<Type>.Ignored)).Returns(null);
            A.CallTo(() => fakeFactories[1].Create(A<Type>.Ignored)).Returns(fakeValidator);
            A.CallTo(() => fakeFactories[2].Create(A<Type>.Ignored)).Returns(null);

            var subject = new DefaultValidatorLocator(fakeFactories);

            var result = subject.GetValidatorForType(typeof(string));

            result.ShouldBeSameAs(fakeValidator);
        }

        [Fact]
        public void Should_return_a_composite_validator_when_more_than_one_factory_can_provide_a_validator_for_the_given_type()
        {
            var fakeFactories = A.CollectionOfFake<IValidatorFactory>(3);
            var fakeValidator1 = A.Fake<IValidator>();
            var fakeValidator2 = A.Fake<IValidator>();
            A.CallTo(() => fakeFactories[1].Create(A<Type>.Ignored)).Returns(fakeValidator1);
            A.CallTo(() => fakeFactories[2].Create(A<Type>.Ignored)).Returns(fakeValidator2);

            var subject = new DefaultValidatorLocator(fakeFactories);

            var result = subject.GetValidatorForType(typeof(string));

            result.ShouldBeOfType<CompositeValidator>();
        }
    }
}