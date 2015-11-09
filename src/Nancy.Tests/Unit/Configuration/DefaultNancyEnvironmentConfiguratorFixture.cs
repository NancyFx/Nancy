namespace Nancy.Tests.Unit.Configuration
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using FakeItEasy;
    using Nancy.Configuration;
    using Xunit;
    using Xunit.Extensions;

    public class DefaultNancyEnvironmentConfiguratorFixture
    {
        private readonly DefaultNancyEnvironmentConfigurator configurator;
        private readonly INancyEnvironmentFactory factory;
        private readonly INancyEnvironment environment;
        private readonly IEnumerable<INancyDefaultConfigurationProvider> defaultConfigurationProviders;

        public DefaultNancyEnvironmentConfiguratorFixture()
        {
            this.environment = A.Fake<INancyEnvironment>();
            this.factory = A.Fake<INancyEnvironmentFactory>();
            this.defaultConfigurationProviders = A.Fake<IEnumerable<INancyDefaultConfigurationProvider>>();

            A.CallTo(() => this.factory.CreateEnvironment()).Returns(this.environment);

            this.configurator = new DefaultNancyEnvironmentConfigurator(this.factory, this.defaultConfigurationProviders);
        }

        [Fact]
        public void Should_retrieve_environment_from_factory()
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

        [Theory]
        [InlineData(1)]
        [InlineData(3)]
        [InlineData(6)]
        [InlineData(8)]
        public void Should_retrieve_default_configurations_from_default_configuration_providers(int numberOfDefaultConfigurationProviders)
        {
            // Given
            var providers = Enumerable
                .Range(1, numberOfDefaultConfigurationProviders)
                .Select(x => A.Fake<INancyDefaultConfigurationProvider>())
                .ToArray();

            var config = new DefaultNancyEnvironmentConfigurator(this.factory, providers);

            // When
            config.ConfigureEnvironment(env => { });

            // Then
            foreach (var provider in providers)
            {
                A.CallTo(() => provider.GetDefaultConfiguration()).MustHaveHappened(Repeated.Exactly.Once);
            }
        }

        [Fact]
        public void Should_not_throw_exception_when_default_configuration_provider_returns_null()
        {
            // Given
            var provider = A.Fake<INancyDefaultConfigurationProvider>();
            A.CallTo(() => provider.GetDefaultConfiguration()).Returns(null);

            var config = new DefaultNancyEnvironmentConfigurator(this.factory, new[] { provider });

            // When, Then
            Assert.DoesNotThrow(() =>
            {
                config.ConfigureEnvironment(env => { });
            });
        }

        [Theory]
        [InlineData(typeof(object))]
        [InlineData(typeof(int))]
        [InlineData(typeof(double))]
        public void Should_get_key_from_configuration_provider_for_default_configuration_object(Type type)
        {
            // Given
            var expectedKey = string.Concat("the-expected-key-", type.Name);
            var env = new DefaultNancyEnvironment();

            var fact = A.Fake<INancyEnvironmentFactory>();
            A.CallTo(() => fact.CreateEnvironment()).Returns(env);

            var provider = A.Fake<INancyDefaultConfigurationProvider>();
            A.CallTo(() => provider.GetDefaultConfiguration()).ReturnsLazily(() => Activator.CreateInstance(type));
            A.CallTo(() => provider.Key).Returns(expectedKey);

            var config = new DefaultNancyEnvironmentConfigurator(fact, new[] { provider });

            // When
            config.ConfigureEnvironment(x => { });

            // Then
            env.ContainsKey(expectedKey).ShouldBeTrue();
        }

        [Fact]
        public void Should_only_add_default_configurations_that_have_not_been_defined_by_user_code()
        {
            // Given
            var key = typeof(object).FullName;

            var env = A.Fake<INancyEnvironment>();
            A.CallTo(() => env.ContainsKey(key)).Returns(true);

            var fact = A.Fake<INancyEnvironmentFactory>();
            A.CallTo(() => fact.CreateEnvironment()).Returns(env);

            var provider = A.Fake<INancyDefaultConfigurationProvider>();
            A.CallTo(() => provider.GetDefaultConfiguration()).Returns(new object());

            var config = new DefaultNancyEnvironmentConfigurator(fact, new[] { provider });

            // When
            config.ConfigureEnvironment(x => { });

            // Then
            A.CallTo(() => env.AddValue(A<string>.That.Matches(x => x.Equals(key)), A<object>._)).MustNotHaveHappened();
        }
    }
}