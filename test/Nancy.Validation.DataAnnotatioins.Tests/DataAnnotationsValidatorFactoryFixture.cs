namespace Nancy.Validation.DataAnnotations.Tests
{
    using FakeItEasy;

    using Nancy.Tests;

    using Xunit;

    public class DataAnnotationsValidatorFactoryFixture
    {
        private readonly DataAnnotationsValidatorFactory subject;

        public DataAnnotationsValidatorFactoryFixture()
        {
            var factory =
                A.Fake<IPropertyValidatorFactory>();

            var adapter =
                A.Fake<IValidatableObjectAdapter>();

            this.subject =
                new DataAnnotationsValidatorFactory(factory, adapter);
        }

        [Fact]
        public void Should_provide_null_validator_when_no_rules_exist()
        {
            // Given, When
            var result = this.subject.Create(typeof(string));

            // Then
            result.ShouldBeNull();
        }
    }
}