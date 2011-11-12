namespace Nancy.Tests.Unit.Validation
{
    using System;
    using FakeItEasy;
    using Nancy.Validation;
    using Xunit;
    using Nancy.Tests.Fakes;

    public class ModuleExtensionsFixture
    {
        private readonly IValidatorLocator validatorLocator;
        private readonly FakeNancyModule subject;

        public ModuleExtensionsFixture()
        {
            validatorLocator = A.Fake<IValidatorLocator>();
            subject = new FakeNancyModule
            {
                ValidatorLocator = validatorLocator
            };
        }

        [Fact]
        public void Should_be_valid_when_no_validator_exists_for_type()
        {
            var result = subject.Validate<FakeModel>(new FakeModel());

            result.IsValid.ShouldBeTrue();
        }

        [Fact]
        public void Should_be_valid_when_a_validator_exists_and_the_instance_is_valid()
        {
            var validator = A.Fake<IValidator>();
            A.CallTo(() => validator.Validate(A<object>.Ignored)).Returns(ValidationResult.Valid);
            A.CallTo(() => validatorLocator.GetValidatorForType(A<Type>.Ignored)).Returns(validator);

            var result = subject.Validate<FakeModel>(new FakeModel());

            result.IsValid.ShouldBeTrue();
        }

        [Fact]
        public void Should_be_invalid_when_a_validator_exists_and_the_instance_is_valid()
        {
            var validator = A.Fake<IValidator>();
            A.CallTo(() => validator.Validate(A<object>.Ignored)).Returns(new ValidationResult(new[] { new ValidationError("blah", s => "blah") }));
            A.CallTo(() => validatorLocator.GetValidatorForType(A<Type>.Ignored)).Returns(validator);

            var result = subject.Validate<FakeModel>(new FakeModel());

            result.IsValid.ShouldBeFalse();
        }

        private class FakeModel
        {
        }
    }
}