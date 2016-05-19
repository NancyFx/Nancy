namespace Nancy.Tests.Unit.Routing
{
    using System.Threading;
    using System.Threading.Tasks;
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
            this.route.Description.Path.ShouldEqual("/test");
        }

        [Fact]
        public async Task Should_set_action_that_returns_not_found_when_instantiated()
        {
            //Given, When
            var response = await this.route.Invoke(new DynamicDictionary(), new CancellationToken());

            // Then
            response.ShouldBeOfType<NotFoundResponse>();
        }
    }
}
