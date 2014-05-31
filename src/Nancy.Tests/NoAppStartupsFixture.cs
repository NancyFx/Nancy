namespace Nancy.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Nancy.Bootstrapper;
    using Nancy.Testing;

    using Xunit;

    public class AutoThingsRegistrations : IRegistrations
    {
        public static bool Throw = false;

        public IEnumerable<TypeRegistration> TypeRegistrations
        {
            get
            {
                if (Throw)
                {
                    throw new Exception();
                }

                return Enumerable.Empty<TypeRegistration>();
            }
        }

        public IEnumerable<CollectionTypeRegistration> CollectionTypeRegistrations
        {
            get
            {
                if (Throw)
                {
                    throw new Exception();
                }

                return Enumerable.Empty<CollectionTypeRegistration>();
            }
        }

        public IEnumerable<InstanceRegistration> InstanceRegistrations
        {
            get
            {
                if (Throw)
                {
                    throw new Exception();
                }

                return Enumerable.Empty<InstanceRegistration>();
            }
        }
    }

    public class NoAppStartupsFixture
    {
        [Fact]
        public void When_AutoRegistration_Is_Enabled_Should_Throw()
        {
            // Enable for tests...
            AutoThingsRegistrations.Throw = true;

            Assert.Throws<Exception>(() =>
            {
                // Given
                var bootstrapper = new ConfigurableBootstrapper(config =>
                {
                    config.Module<NoAppStartupsModule>();
                    config.Dependency<INoAppStartupsTestDependency>(typeof(AutoDependency));
                });
                var browser = new Browser(bootstrapper);

                // When
                browser.Get("/");
            });
        }

        [Fact]
        public void When_AutoRegistration_Is_Disabled_Should_Not_Throw()
        {
            // Enable for tests...
            AutoThingsRegistrations.Throw = true;

            // Given
            var bootstrapper = new ConfigurableBootstrapper(config =>
            {
                config.DisableAutoRegistrations();
                config.Module<NoAppStartupsModule>();
                config.Dependency<INoAppStartupsTestDependency>(typeof(AutoDependency));
            });
            var browser = new Browser(bootstrapper);

            // When
            var result = browser.Get("/");

            // Then
            result.Body.AsString().ShouldEqual("disabled auto registration works");
        }

        public interface INoAppStartupsTestDependency
        {
            string GetStuff();
        }

        public class AutoDependency : INoAppStartupsTestDependency
        {
            public string GetStuff()
            {
                return "disabled auto registration works";
            }
        }

        public class NoAppStartupsModule : NancyModule
        {
            public NoAppStartupsModule(INoAppStartupsTestDependency dependency)
            {
                this.Get["/"] = _ => dependency.GetStuff();
            }
        }
    }
}
