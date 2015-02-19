namespace Nancy.Tests.Unit.Routing
{
    using Nancy.Testing;

    using Xunit;

    public class ConstraintNodeRouteScoringFixture
    {
        private readonly Browser browser;

        public ConstraintNodeRouteScoringFixture()
        {
            this.browser = new Browser(with => with.Module<TestModule>());
        }

        [Fact]
        public void Should_return_constraint_route_when_satisfying_the_constraint()
        {
            var result = this.browser.Get("/123");

            result.Body.AsString().ShouldEqual("constraint");
        }

        [Fact]
        public void Should_return_normal_capture_route_when_constraint_is_not_satisfied()
        {
            var result = this.browser.Get("/banana");

            result.Body.AsString().ShouldEqual("capture");
        }

        private class TestModule : NancyModule
        {
            public TestModule()
            {
                Get["{value:int}"] = _ => "constraint";
                Get["{value}"] = _ => "capture";
            }
        }
    }
}