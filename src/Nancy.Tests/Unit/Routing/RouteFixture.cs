namespace Nancy.Tests.Unit.Routing
{
    using System;
    using System.Threading;

    using Nancy.Routing;

    using Xunit;

    public class RouteFixture
    {
        [Fact]
        public void Should_throw_argumentexception_when_instantiated_with_null_method()
        {
            //Given, When
            var exception =
                Record.Exception(() => new Route(null, "", null, (x, c) => null));

            // Then
            exception.ShouldBeOfType<ArgumentException>();
        }

        [Fact]
        public void Should_throw_argumentexception_when_instantiated_with_empty_method()
        {
            //Given, When
            var exception =
                Record.Exception(() => new Route("", "/", null, (x,c) => null));

            // Then
            exception.ShouldBeOfType<ArgumentException>();
        }

        [Fact]
        public void Should_throw_argumentexception_when_instantiated_with_null_path()
        {
            //Given, When
            var exception =
                Record.Exception(() => new Route("GET", null, null, (x, c) => null));

            // Then
            exception.ShouldBeOfType<ArgumentException>();
        }

        [Fact]
        public void Should_throw_argumentexception_when_instantiated_with_empty_path()
        {
            //Given, When
            var exception =
                Record.Exception(() => new Route("GET", null, null, (x, c) => null));

            // Then
            exception.ShouldBeOfType<ArgumentException>();
        }

        [Fact]
        public void Should_throw_argumentnullexception_when_instantiated_with_null_action()
        {
            //Given, When
            var exception =
                Record.Exception(() => new Route("GET", "/", null, null));

            // Then
            exception.ShouldBeOfType<ArgumentNullException>();
        }

        [Fact]
        public void Should_invoke_action_with_parameters_when_invoked()
        {
            //Given
            DynamicDictionary capturedParameters = null;

            Func<dynamic, Response> action = x =>
            {
                capturedParameters = x;
                return null;
            };

            dynamic parameters = new DynamicDictionary();
            parameters.foo = 10;
            parameters.bar = "value";

            var route = Route.FromSync("GET", "/", null, action);

            // When
            route.Invoke(parameters, new CancellationToken());

            // Then
            capturedParameters.ShouldBeSameAs((object)parameters);
        }

        [Fact]
        public void Should_return_response_from_action_when_invoked()
        {
            //Given
            var expectedResponse = new Response();
            Func<object, Response> action = x => expectedResponse;

            var route = Route.FromSync("GET", "/", null, action);

            // When
            var response = (Response)route.Invoke(new DynamicDictionary(), new CancellationToken()).Result;

            // Then
            response.ShouldBeSameAs(expectedResponse);
        }
    }
}