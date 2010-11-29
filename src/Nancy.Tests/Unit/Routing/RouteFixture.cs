namespace Nancy.Tests.Unit.Routing
{
    using System;
    using Nancy.Routing;
    using Xunit;

    public class RouteFixture
    {
        [Fact]
        public void Should_throw_argumentnullexception_when_instantiated_with_null_path()
        {
            //Given, When
            var exception =
                Record.Exception(() => new Route(null, null, x => null));

            // Then
            exception.ShouldBeOfType<ArgumentNullException>();
        }

        [Fact]
        public void Should_throw_argumentnullexception_when_instantiated_with_null_action()
        {
            //Given, When
            var exception =
                Record.Exception(() => new Route("/", null, null));

            // Then
            exception.ShouldBeOfType<ArgumentNullException>();
        }

        [Fact]
        public void Should_set_path_property_when_instantiated()
        {
            //Given, When
            const string path = "/dummy/path";
            var route = new Route(path, null, x => null);

            // Then
            route.Path.ShouldEqual(path);
        }

        [Fact]
        public void Should_set_action_property_when_instantiated()
        {
            //Given, When
            Func<dynamic, Response> action = x => null;
            var route = new Route("/", null, action);

            // Then
            route.Action.ShouldBeSameAs(action);
        }

        [Fact]
        public void Should_set_paramters_property_when_instantiated()
        {
            //Given, When
            Func<dynamic, Response> action = x => null;

            dynamic parameters = new RouteParameters();
            parameters.foo = 10;
            parameters.bar = "value";

            var route = new Route("/", parameters, action);

            // Then
            ((object)route.Parameters).ShouldBeSameAs((object)parameters);
        }

        [Fact]
        public void Should_invoke_action_with_parameters_when_invoked()
        {
            //Given
            RouteParameters capturedParameters = null;

            Func<dynamic, Response> action = x => {
                capturedParameters = x;
                return null;
            };

            dynamic parameters = new RouteParameters();
            parameters.foo = 10;
            parameters.bar = "value";

            var route = new Route("/", parameters, action);

            // When
            route.Invoke();

            // Then
            capturedParameters.ShouldBeSameAs((object)parameters);
        }

        [Fact]
        public void Should_return_response_from_action_when_invoked()
        {
            //Given
            var expectedResponse = new Response();
            Func<object, Response> action = x => expectedResponse;

            var route = new Route("/", null, action);

            // When
            var response = route.Invoke();

            // Then
            response.ShouldBeSameAs(expectedResponse);
        }
    }
}