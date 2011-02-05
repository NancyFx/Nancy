namespace Nancy.Tests.Unit.Routing
{
    using Nancy.Routing;
    using Xunit;

    public class NotFoundRouteFixture
    {
        private readonly IRoute route;

        public NotFoundRouteFixture()
        {
            this.route = new NotFoundRoute("/test");
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