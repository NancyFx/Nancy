namespace Nancy.Tests.Unit.Routing
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Nancy.Routing;
    using Xunit;

    public class RouteFixture
    {
        [Fact]
        public void Should_throw_argumentexception_when_instantiated_with_null_method()
        {
            //Given, When
            var exception =
                Record.Exception(() =>
                {
                    return new Route<object>(null, "", null, x => true, (args, ct) => null);
                });

            // Then
            exception.ShouldBeOfType<ArgumentException>();
        }

        [Fact]
        public void Should_throw_argumentexception_when_instantiated_with_empty_method()
        {
            //Given, When
            var exception =
                Record.Exception(() =>
                {
                    return new Route<object>("", "/", null, x => true, (args, ct) => null);
                });

            // Then
            exception.ShouldBeOfType<ArgumentException>();
        }

        [Fact]
        public void Should_throw_argumentexception_when_instantiated_with_null_path()
        {
            //Given, When
            var exception =
                Record.Exception(() =>
                {
                    return new Route<object>("GET", null, null, x => true, (args, ct) => null);
                });

            // Then
            exception.ShouldBeOfType<ArgumentException>();
        }

        [Fact]
        public void Should_throw_argumentexception_when_instantiated_with_empty_path()
        {
            //Given, When
            var exception =
                Record.Exception(() =>
                {
                    new Route<object>("GET", null, null, x => true, (args, ct) => null);
                });

            // Then
            exception.ShouldBeOfType<ArgumentException>();
        }

        [Fact]
        public void Should_invoke_action_with_parameters_when_invoked()
        {
            //Given
            DynamicDictionary capturedParameters = null;

            Func<dynamic, CancellationToken, Task<object>> action = (args, ct) =>
            {
                capturedParameters = args;
                return Task.FromResult<object>(null);
            };

            dynamic parameters = new DynamicDictionary();
            parameters.foo = 10;
            parameters.bar = "value";

            var route = new Route<object>("GET", "/", null, action);

            // When
            route.Invoke(parameters, new CancellationToken());

            // Then
            capturedParameters.ShouldBeSameAs((object)parameters);
        }

        [Fact]
        public async Task Should_return_response_from_action_when_invoked()
        {
            //Given
            var expectedResponse = new Response();
            Func<dynamic, CancellationToken, Task<Response>> action = (args, ct) => Task.FromResult(expectedResponse);

            var route = new Route<Response>("GET", "/", null, action);

            // When
            var response = await route.Invoke(new DynamicDictionary(), new CancellationToken());

            // Then
            response.ShouldBeSameAs(expectedResponse);
        }
    }
}