namespace Nancy.Tests.Unit.Routing
{
    using Nancy.Routing;
    using Xunit;

    public class NotFoundRouteFixture
    {
        private readonly Route route;

        public NotFoundRouteFixture()
        {
            this.route = new NotFoundRoute("GET", "/test");
        }

        [Fact]
        public void Should_set_route_property_when_instantiated()
        {
            //Given, When, Then
            route.Description.Path.ShouldEqual("/test");
        }

        [Fact]
        public void Should_set_action_that_returns_not_found_when_instantiated()
        {
            //Given, When
            var response = route.Invoke(new DynamicDictionary());

            // Then
            response.ShouldBeOfType<NotFoundResponse>();
        }
    }
}