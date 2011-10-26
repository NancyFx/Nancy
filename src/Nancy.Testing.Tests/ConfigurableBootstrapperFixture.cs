namespace Nancy.Testing.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    using FakeItEasy;
    using Nancy.Bootstrapper;
    using Nancy.Tests;
    using Xunit;
    using Xunit.Sdk;

    public class ConfigurableBootstrapperFixture
    {
        [Fact]
        public void Should_use_default_type_when_no_type_or_instance_overrides_have_been_configured()
        {
            // Given
            var bootstrapper = new ConfigurableBootstrapper();
            bootstrapper.Initialise();

            // When
            var engine = bootstrapper.GetEngine();

            // Then
            engine.ShouldBeOfType<NancyEngine>();
        }

        [Fact]
        public void Should_use_type_override_when_it_has_been_configured()
        {
            // Given
            var bootstrapper = new ConfigurableBootstrapper(with =>
            {
                with.NancyEngine<FakeNancyEngine>();
            });

            bootstrapper.Initialise();

            // When
            var engine = bootstrapper.GetEngine();

            // Then
            engine.ShouldBeOfType<FakeNancyEngine>();
        }

        [Fact]
        public void Should_use_instance_override_when_it_has_been_configured()
        {
            // Given
            var fakeEngine = A.Fake<INancyEngine>();

            var bootstrapper = new ConfigurableBootstrapper(with =>
            {
                with.NancyEngine(fakeEngine);
            });

            bootstrapper.Initialise();

            // When
            var engine = bootstrapper.GetEngine();

            // Then
            engine.ShouldBeSameAs(fakeEngine);
        }

        [Fact]
        public void Should_use_instance_override_when_both_type_and_instance_overrides_have_been_configured()
        {
            // Given
            var fakeEngine = A.Fake<INancyEngine>();

            var bootstrapper = new ConfigurableBootstrapper(with =>
            {
                with.NancyEngine<FakeNancyEngine>();
                with.NancyEngine(fakeEngine);
            });

            bootstrapper.Initialise();

            // When
            var engine = bootstrapper.GetEngine();

            // Then
            engine.ShouldBeSameAs(fakeEngine);
        }

        [Fact]
        public void Should_provide_configuration_for_all_base_properties()
        {
            // Given
            var availableMembers =
                typeof(ConfigurableBootstrapper.ConfigurableBoostrapperConfigurator)
                .GetMethods(BindingFlags.Public | BindingFlags.Instance)
                .Select(x => x.Name)
                .Distinct();

            var expectedConfigurableMembers =
                this.GetConfigurableBootstrapperMembers();

            // When
            var result = expectedConfigurableMembers.Where(x => !availableMembers.Contains(x, StringComparer.OrdinalIgnoreCase)).ToArray();

            // Then
            if (result.Any())
            {
                throw new AssertException(string.Format("Types missing from configurable versions: {0} ", result.Aggregate((t1, t2) => t1 + ", " + t2)));
            }
        }

        public IEnumerable<string> GetConfigurableBootstrapperMembers()
        {
            var ignoreList = new[]
            {
                "AfterRequest", "BeforeRequest", "IsValid", "ModuleKeyGenerator",
                "BindingDefaults", "OnError"
            };

            var typesToReflect =
                new[] { typeof(NancyBootstrapperBase<>), typeof(NancyInternalConfiguration) };

            return typesToReflect
                .SelectMany(x => x.GetProperties(BindingFlags.Public | BindingFlags.Instance))
                .Select(x => x.Name)
                .Where(x => !ignoreList.Contains(x, StringComparer.OrdinalIgnoreCase));
        }

        private class FakeNancyEngine : INancyEngine
        {
            public Func<NancyContext, Response> PreRequestHook { get; set; }

            public Action<NancyContext> PostRequestHook { get; set; }

            public Func<NancyContext, Exception, Response> OnErrorHook { get; set; }

            public Func<NancyContext, IPipelines> RequestPipelinesFactory { get; set; }

            public NancyContext HandleRequest(Request request)
            {
                throw new NotImplementedException();
            }

            public void HandleRequest(Request request, Action<NancyContext> onComplete, Action<Exception> onError)
            {
                throw new NotImplementedException();
            }
        }
    }
}