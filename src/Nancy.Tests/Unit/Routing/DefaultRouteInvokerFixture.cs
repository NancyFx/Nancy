namespace Nancy.Tests.Unit.Routing
{
    using System.Linq;
    using FakeItEasy;
    using Fakes;
    using Nancy.Routing;
    using Xunit;

    public class DefaultRouteInvokerFixture
    {
        private readonly DefaultRouteInvoker invoker;

        public DefaultRouteInvokerFixture()
        {
            this.invoker = new DefaultRouteInvoker(Enumerable.Empty<ISerializer>());
        }

        [Fact]
        public void Should_invoke_route_with_provided_parameters()
        {
            // Given
            var parameters = new DynamicDictionary();
            var route = new FakeRoute();
            var context = new NancyContext();

            // When
            this.invoker.Invoke(route, parameters, context);

            // Then
            Assert.Same(route.ParametersUsedToInvokeAction, parameters);
        }
    }
}