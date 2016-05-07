namespace Nancy.Tests.Unit.Routing
{
    using System.Threading.Tasks;
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
        public async Task Should_return_constraint_route_when_satisfying_the_constraint()
        {
            var result = await this.browser.Get("/123");

            result.Body.AsString().ShouldEqual("constraint");
        }

        [Fact]
        public async Task Should_return_normal_capture_route_when_constraint_is_not_satisfied()
        {
            var result = await this.browser.Get("/banana");

            result.Body.AsString().ShouldEqual("capture");
        }

        private class TestModule : NancyModule
        {
            public TestModule()
            {
                Get("{value:int}", args => "constraint");
                Get("{value}", args => "capture");
            }
        }
    }
}