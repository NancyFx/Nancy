namespace Nancy.Tests.Unit.Validation
{
    using System;

    using FakeItEasy;

    using Nancy.Tests.Fakes;
    using Nancy.Validation;

    using Xunit;

    public class ModuleExtensionsFixture
    {
        private readonly NancyContext context;
        private readonly IModelValidatorLocator validatorLocator;
        private readonly FakeNancyModule subject;

        public ModuleExtensionsFixture()
        {
            this.context = new NancyContext();
            this.validatorLocator = A.Fake<IModelValidatorLocator>();
            this.subject = new FakeNancyModule
            {
                Context = this.context,
                ValidatorLocator = this.validatorLocator
            };
        }

        [Fact]
        public void Should_be_valid_when_no_validator_exists_for_type()
        {
            // Given, When
            var result = subject.Validate<FakeModel>(new FakeModel());

            // Then
            result.IsValid.ShouldBeTrue();
        }

        [Fact]
        public void Should_be_valid_when_a_validator_exists_and_the_instance_is_valid()
        {
            // Given
            var validator = A.Fake<IModelValidator>();
            A.CallTo(() => validator.Validate(A<object>.Ignored, A<NancyContext>._)).Returns(new ModelValidationResult());
            A.CallTo(() => validatorLocator.GetValidatorForType(A<Type>.Ignored)).Returns(validator);

            // When
            var result = subject.Validate(new FakeModel());

            // Then
            result.IsValid.ShouldBeTrue();
        }

        [Fact]
        public void Should_be_invalid_when_a_validator_exists_and_the_instance_is_valid()
        {
            // Given
            var validator = A.Fake<IModelValidator>();
            A.CallTo(() => validator.Validate(A<object>.Ignored, A<NancyContext>._)).Returns(new ModelValidationResult(new[] { new ModelValidationError("blah", "blah") }));
            A.CallTo(() => validatorLocator.GetValidatorForType(A<Type>.Ignored)).Returns(validator);

            // When
            var result = subject.Validate(new FakeModel());

            // Then
            result.IsValid.ShouldBeFalse();
        }

        [Fact]
        public void Should_pass_context_to_validator()
        {
            // Given
            var validator = A.Fake<IModelValidator>();
            A.CallTo(() => validatorLocator.GetValidatorForType(A<Type>.Ignored)).Returns(validator);

            // When
            subject.Validate(new FakeModel());

            // Then
            A.CallTo(() => validator.Validate(A<object>._, this.context)).MustHaveHappened();
        }

        [Fact]
        public void Should_pass_model_instance_to_validator()
        {
            // Given
            var model = new FakeModel();
            var validator = A.Fake<IModelValidator>();
            A.CallTo(() => validatorLocator.GetValidatorForType(A<Type>.Ignored)).Returns(validator);

            // When
            subject.Validate(model);

            // Then
            A.CallTo(() => validator.Validate(model, A<NancyContext>._)).MustHaveHappened();
        }

        [Fact]
        public void Should_add_errors_to_existing_module_errors()
        {
            //Given
            subject.ModelValidationResult =
                new ModelValidationResult(new[] { new ModelValidationError("FirstName", "Please enter a name") });
            
            var model = new FakeModel();
            
            var validator = A.Fake<IModelValidator>();
            
            A.CallTo(() => validatorLocator.GetValidatorForType(A<Type>.Ignored)).Returns(validator);
            
            A.CallTo(() => validator.Validate(model, A<NancyContext>._))
             .Returns(
                 new ModelValidationResult(new[] { new ModelValidationError("LastName", "Please enter a last name") }));

            // When
            subject.Validate(model);

            // Then
            subject.ModelValidationResult.Errors.ShouldHaveCount(2);
        }

        private class FakeModel
        {
        }
    }
}