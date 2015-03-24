namespace Nancy.Tests.Functional.Tests
{
    using System;

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
                    configuration.RouteSegmentConstraints(new[] { typeof(UltimateRouteSegmentConstraint), typeof(VersionRouteSegmentConstraint) });
                });

            this.browser = new Browser(this.bootstrapper);
        }

        [Fact]
        public void multiple_parameters_per_segment_should_support_constraints()
        {
            // Given
            const string url = @"/42...42";

            // When
            var response = this.browser.Get(
                url,
                with =>
                {
                    with.HttpRequest();
                });

            // Then
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.True(Invoked);
        }

        [Fact]
        public void versionsegmentrouteconstraint_should_match_on_version_numbers()
        {
            // Given
            const string url = @"/4.1.2...4.1.5";

            // When
            var response = this.browser.Get(
                url,
                with =>
                {
                    with.HttpRequest();
                    with.Accept("application/json");
                });

            // Then
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Equal(response.Body.AsString(), "{\"left\":\"4.1.2\",\"right\":\"4.1.5\"}");
        }


        public class UltimateRouteSegmentConstraint : RouteSegmentConstraintBase<int>
        {
            public override string Name
            {
                get { return "correctanswer"; }
            }

            protected override bool TryMatch(string constraint, string segment, out int matchedValue)
            {
                Invoked = true;
                if (int.TryParse(segment, out matchedValue))
                {
                    return matchedValue == 42;
                }
                return false;
            }
        }
    }

    public class RouteConstraintsModule : NancyModule
    {
        public RouteConstraintsModule()
        {
            this.Get["/{left:correctanswer}...{right:correctanswer}"] = _ =>
            {
                return HttpStatusCode.OK;
            };

            // For testing VersionSegmentRouteConstraint
            this.Get["/{left:version}...{right:version}"] = _ =>
            {
                return new {_.left, _.right};
            };

        
        }
    }
}
