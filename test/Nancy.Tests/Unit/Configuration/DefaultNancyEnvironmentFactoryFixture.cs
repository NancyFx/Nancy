namespace Nancy.Tests.Unit.Configuration
{
    using Nancy.Configuration;
    using Xunit;

    public class DefaultNancyEnvironmentFactoryFixture
    {
        [Fact]
        public void Should_return_instance_of_default_environment()
        {
            // Given
            var factory = new DefaultNancyEnvironmentFactory();

            // When
            var result = factory.CreateEnvironment();

            // Then
            result.ShouldBeOfType<DefaultNancyEnvironment>();
        }
    }
}