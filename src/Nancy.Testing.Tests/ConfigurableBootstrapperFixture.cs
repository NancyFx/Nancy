namespace Nancy.Testing.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Threading;
    using System.Threading.Tasks;

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
                typeof(ConfigurableBootstrapper.ConfigurableBootstrapperConfigurator)
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

        [Fact]
        public void Should_throw_exceptions_if_any_occur_in_route()
        {
            var bootstrapper = new ConfigurableBootstrapper(with =>
                {
                    with.Module<BlowUpModule>();
                });
            bootstrapper.Initialise();
            var engine = bootstrapper.GetEngine();
            var request = new Request("GET", "/", "http");

            var result = Record.Exception(() => engine.HandleRequest(request));

            result.ShouldNotBeNull();
            result.ShouldBeOfType<Exception>();
            result.ToString().ShouldContain("Oh noes!");
        }

        [Fact]
        public void Should_run_application_startup_closure()
        {
            var date = new DateTime(2112,10,31);

            var bootstrapper = new ConfigurableBootstrapper(with => 
            {
                with.ApplicationStartup((container, pipelines) =>
                {
                    pipelines.BeforeRequest += ctx =>
                        {
                            ctx.Items.Add("date", date);
                            return null;
                        };
                });
            });

            bootstrapper.Initialise();

            var engine = bootstrapper.GetEngine();
            var request = new Request("GET", "/", "http");
            var result = engine.HandleRequest(request);

            result.Items["date"].ShouldEqual(date);
        }

        [Fact]
        public void Should_run_request_startup_closure()
        {
            var date = new DateTime(2112, 10, 31);
            var bootstrapper =
                new ConfigurableBootstrapper(
                    with => with.RequestStartup((container, pipelines, context) => 
                        context.Items.Add("date", date)));

            bootstrapper.Initialise();

            var engine = bootstrapper.GetEngine();
            var request = new Request("GET", "/", "http");
            var result = engine.HandleRequest(request);

            result.Items["date"].ShouldEqual(date);
        }

        public IEnumerable<string> GetConfigurableBootstrapperMembers()
        {
            var ignoreList = new[]
            {
                "AfterRequest", "BeforeRequest", "IsValid", "ModuleKeyGenerator",
                "BindingDefaults", "OnError", "InteractiveDiagnosticProviders", "RequestTracing", "IgnoredAssemblies"
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

            public Func<NancyContext, Exception, dynamic> OnErrorHook { get; set; }

            public Func<NancyContext, IPipelines> RequestPipelinesFactory { get; set; }

            public Task<NancyContext> HandleRequest(Request request, Func<NancyContext, NancyContext> preRequest, CancellationToken cancellationToken)
            {
                throw new NotImplementedException();
            }

            public void Dispose()
            {
            }
        }

        private class BlowUpModule : NancyModule
        {
            public BlowUpModule()
            {
                Get["/"] = _ => { throw new InvalidOperationException("Oh noes!"); };
            }
        }
    }
}