namespace Nancy.Tests.Unit.Configuration
{
    using System;
    using FakeItEasy;
    using Nancy.Configuration;
    using Xunit;

    public class DefaultNancyEnvironmentConfiguratorFixture
    {
        private readonly DefaultNancyEnvironmentConfigurator configurator;
        private readonly INancyEnvironmentFactory factory;
        private readonly INancyEnvironment environment;

        public DefaultNancyEnvironmentConfiguratorFixture()
        {
            this.environment = A.Fake<INancyEnvironment>();
            this.factory = A.Fake<INancyEnvironmentFactory>();

            A.CallTo(() => this.factory.CreateEnvironment()).Returns(this.environment);

            this.configurator = new DefaultNancyEnvironmentConfigurator(this.factory);
        }

        [Fact]
        public void Should_retreive_environment_from_factory()
        {
            // Given, When
            this.configurator.ConfigureEnvironment(env => { });

            // Then
            A.CallTo(() => this.factory.CreateEnvironment()).MustHaveHappened(Repeated.Exactly.Once);
        }

        [Fact]
        public void Should_call_configuration_with_instance_from_factory()
        {
            // Given
            INancyEnvironment passedEnvironmentInstance = null;

            Action<INancyEnvironment> configuration = env =>
            {
                passedEnvironmentInstance = env;
            };

            // When
            this.configurator.ConfigureEnvironment(configuration);

            // Then
            passedEnvironmentInstance.ShouldBeSameAs(this.environment);
        }

        [Fact]
        public void Should_return_instance_from_factory()
        {
            // Given, When
            var result = this.configurator.ConfigureEnvironment(env => { });

            // Then
            result.ShouldBeSameAs(this.environment);
        }
    }
}