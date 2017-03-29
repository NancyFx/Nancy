namespace Nancy.Validation.DataAnnotations.Tests
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;

    using Nancy.Tests;

    using Xunit;

    public class DefaultValidatableObjectAdapterFixture
    {
        public readonly DefaultValidatableObjectAdapter validator;

        public DefaultValidatableObjectAdapterFixture()
        {
            this.validator = new DefaultValidatableObjectAdapter();
        }

        [Fact]
        public void Should_invoke_validate_on_instance()
        {
            // Given
            var instance = new ModelUnderTest();

            // When
            this.validator.Validate(instance, new NancyContext());

            // Then
            instance.ValidatedWasInvoked.ShouldBeTrue();
        }

        [Fact]
        public void Should_invoke_validate_with_instance()
        {
            // Given
            var instance = new ModelUnderTest();

            // When
            this.validator.Validate(instance, new NancyContext());

            // Then
            instance.InstanceBeingValidated.ShouldBeSameAs(instance);
        }

        [Fact]
        public void Should_return_validation_error_for_all_validation_results()
        {
            // Given
            var result1 = new ValidationResult("Error1", new[] { "foo" });
            var result2 = new ValidationResult("Error2", new[] { "bar", "baz" });

            var instance = new ModelUnderTest
            {
                ExpectedResults = new[] {result1, result2}
            };

            // When
            var results = this.validator.Validate(instance, new NancyContext());

            // Then
            results.Count().ShouldEqual(2);
        }

        [Fact]
        public void Should_not_return_errors_if_model_not_implements_IValidatableObject()
        {
            // Given
            var instance = new ModelNotImplementingIValidatableObject();

            // When
            var result = this.validator.Validate(instance, new NancyContext());

            // Then
            result.Count().ShouldEqual(0);
        }

        public class ModelUnderTest : IValidatableObject
        {
            public object InstanceBeingValidated { get; set; }

            public bool ValidatedWasInvoked { get; set; }

            public IEnumerable<ValidationResult> ExpectedResults { get; set; }

            public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
            {
                this.ValidatedWasInvoked = true;
                this.InstanceBeingValidated = validationContext.ObjectInstance;
                
                return this.ExpectedResults ?? Enumerable.Empty<ValidationResult>();
            }
        }

        public class ModelNotImplementingIValidatableObject
        {
            public int Value { get; set; }
        }
    }
}