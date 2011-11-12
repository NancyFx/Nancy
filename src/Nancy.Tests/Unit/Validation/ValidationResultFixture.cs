namespace Nancy.Tests.Unit.Validation
{
    using System;
    using FakeItEasy;
    using Nancy.Validation;
    using Xunit;

    public class ValidationResultFixture
    {
        [Fact]
        public void Should_not_throw_if_null_errors_collection_is_passed()
        {
            var result = Record.Exception(() => new ValidationResult(null));

            result.ShouldBeNull();
        }

        [Fact]
        public void Should_return_valid_when_no_errors_exist()
        {
            var subject = new ValidationResult(null);

            var result = subject.IsValid;

            result.ShouldBeTrue();
        }

        [Fact]
        public void Should_return_invalid_if_any_errors_exist()
        {
            var subject = new ValidationResult(new[] { new ValidationError("blah", s => "blah") });

            var result = subject.IsValid;

            result.ShouldBeFalse();
        }
    }
}