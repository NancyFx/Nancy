namespace Nancy.Validation.DataAnnotations.Tests
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;

    using Nancy.Tests;
    using Nancy.Validation.DataAnnotations;

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
            this.validator.Validate(instance);

            // Then
            instance.ValidatedWasInvoked.ShouldBeTrue();
        }

        [Fact]
        public void Should_invoke_validate_with_instance()
        {
            // Given
            var instance = new ModelUnderTest();

            // When
            this.validator.Validate(instance);

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
            var results = this.validator.Validate(instance);

            // Then
            results.Count().ShouldEqual(2);
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
    }
}