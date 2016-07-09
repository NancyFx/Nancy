namespace Nancy.Tests.Unit.Validation
{
    using System.Collections.Generic;

    using Nancy.Validation;

    using Xunit;

    public class ModelValidationResultFixture
    {
        [Fact]
        public void Should_not_throw_if_null_errors_collection_is_passed()
        {
            // Given, When
            var result = Record.Exception(() => new ModelValidationResult((IEnumerable<ModelValidationError>)null));

            // Then
            result.ShouldBeNull();
        }

        [Fact]
        public void Should_return_valid_when_no_errors_exist()
        {
            // Given
            var subject = new ModelValidationResult((IEnumerable<ModelValidationError>)null);

            // When
            var result = subject.IsValid;

            // Then
            result.ShouldBeTrue();
        }

        [Fact]
        public void Should_return_invalid_if_any_errors_exist()
        {
            // Given
            var subject = new ModelValidationResult(new[] { new ModelValidationError("blah", "blah") });

            // When
            var result = subject.IsValid;

            // Then
            result.ShouldBeFalse();
        }
    }
}