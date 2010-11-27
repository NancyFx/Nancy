namespace Nancy.Tests.Unit.Routing
{
    using Nancy.Routing;
    using Xunit;

    public class NoMatchingRouteFixture
    {
        public readonly IRoute route;

        public NoMatchingRouteFixture()
        {
            this.route = new NoMatchingRouteFoundRoute("/test");
        }

        [Fact]
        public void Should_set_route_property_when_instantiated()
        {
            //Given, When, Then
            route.Path.ShouldEqual("/test");
        }

        [Fact]
        public void Should_set_action_that_returns_not_found_when_instantiated()
        {
            //Given, When
            var response = route.Invoke();

            // Then
            response.ShouldBeOfType<NotFoundResponse>();
        }
    }
}