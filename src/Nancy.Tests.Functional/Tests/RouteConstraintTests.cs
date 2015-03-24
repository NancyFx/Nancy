using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nancy.Tests.Functional.Tests
{
    using Nancy.Bootstrapper;
    using Nancy.Routing.Constraints;
    using Nancy.Testing;

    using Xunit;

    public class RouteConstraintTests
    {
        private readonly INancyBootstrapper bootstrapper;

        private readonly Browser browser;

        public static bool Invoked { get; set; }

        public RouteConstraintTests()
        {
            Invoked = false;

            this.bootstrapper = new ConfigurableBootstrapper(
                configuration =>
                {
                    configuration.ApplicationStartup((c, p) => { });
                    configuration.Modules(new Type[] { typeof(RouteConstraintsModule) });
                    configuration.RouteSegmentConstraints(new[] { typeof(VersionRouteConstraint) });
                });

            this.browser = new Browser(this.bootstrapper);
        }

        [Fact]
        public void multiple_parameters_per_segment_should_support_constraints()
        {
            var response = browser.Get(
                @"/4.3...5.3",
                with =>
                {
                    with.HttpRequest();
                });

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.True(Invoked);
        }


        public class VersionRouteConstraint : RouteSegmentConstraintBase<Version>
        {
            public override string Name
            {
                get { return "version"; }
            }

            protected override bool TryMatch(string constraint, string segment, out Version matchedValue)
            {
                Invoked = true;
                return Version.TryParse(segment, out matchedValue);
            }
        }
    }

    public class RouteConstraintsModule : NancyModule
    {
        public RouteConstraintsModule()
        {
            this.Get["/{left:version}...{right:version}"] = _ =>
            {
                return HttpStatusCode.OK;
            };

        
        }
    }
}
