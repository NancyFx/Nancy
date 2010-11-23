namespace Nancy.Tests.Unit
{
    using System;
    using System.Linq;
    using System.Net;
    using FakeItEasy;
    using Nancy;
    using Nancy.Tests.Fakes;
    using Xunit;

    public class NancyEngineFixture
    {
        private readonly INancyEngine engine;
        private readonly IRouteResolver resolver;
        private readonly INancyModuleLocator locator;

        public NancyEngineFixture()
        {
            this.locator = A.Fake<INancyModuleLocator>();
            this.resolver = A.Fake<IRouteResolver>();
            this.engine = new NancyEngine(this.locator, this.resolver);
        }

        [Fact]
        public void Should_throw_argumentnullexception_when_created_with_null_locator()
        {
            // Arrange, Act
            var exception =
                Catch.Exception(() => new NancyEngine(null, A.Fake<IRouteResolver>()));

            // Assert
            exception.ShouldBeOfType<ArgumentNullException>();
        }

        [Fact]
        public void Should_throw_argumentnullexception_when_created_with_null_resolver()
        {
            // Arrange, Act
            var exception =
                Catch.Exception(() => new NancyEngine(A.Fake<INancyModuleLocator>(), null));

            // Assert
            exception.ShouldBeOfType<ArgumentNullException>();
        }

        [Fact]
        public void Should_retrieve_modules_from_locator_when_handling_request()
        {
            var request =
                new Request("GET", new Uri("http://localhost"));

            var response =
                this.engine.HandleRequest(request);

            A.CallTo(() => this.locator.GetModules()).MustHaveHappened();
        }

        [Fact]
        public void Should_return_not_found_response_when_no_nancy_modules_could_be_found()
        {
            // Arrange
            A.CallTo(() => this.locator.GetModules()).Returns(Enumerable.Empty<INancy>());

            var request =
                new Request("GET", new Uri("http://localhost"));

            // Act
            var response =
                this.engine.HandleRequest(request);

            // Assert
            response.StatusCode.ShouldEqual(HttpStatusCode.NotFound);
        }

        [Fact]
        public void Should_call_route_resolver_with_request_and_retrieved_modules()
        {
            // Arrange
            var modules = new [] { new FakeNancy() };
            var request = new Request("GET", new Uri("http://localhost"));

            A.CallTo(() => this.locator.GetModules()).Returns(modules);

            // Act
            var response = this.engine.HandleRequest(request);

            // Assert
            A.CallTo(() => this.resolver.GetRoute(request, modules)).MustHaveHappened();
        }

        [Fact]
        public void Should_throw_argumentnullexception_when_handling_null_request()
        {
            // Arrange, Act
            var exception =
                Catch.Exception(() => this.engine.HandleRequest(null));

            // Assert
            exception.ShouldBeOfType<ArgumentNullException>();
        }

        [Fact]
        public void Should_invoke_resolved_route()
        {
            // Arrange
            var route = A.Fake<IRoute>();

            var modules = new[] { new FakeNancy() };
            var request = new Request("GET", new Uri("http://localhost"));

            A.CallTo(() => this.locator.GetModules()).Returns(modules);
            A.CallTo(() => this.resolver.GetRoute(request, modules)).Returns(route);

            // Act
            var response = this.engine.HandleRequest(request);

            // Assert
            A.CallTo(() => route.Invoke()).MustHaveHappened();
        }

        [Fact]
        public void Should_return_not_found_response_when_no_route_was_matched()
        {
            // Arrange
            var request = new Request("GET", new Uri("http://localhost"));
            var modules = new[] { new FakeNancy() };

            A.CallTo(() => this.locator.GetModules()).Returns(modules);
            A.CallTo(() => this.resolver.GetRoute(request, modules)).Returns(null);

            // Act
            var response = this.engine.HandleRequest(request);

            // Assert
            response.StatusCode.ShouldEqual(HttpStatusCode.NotFound);
        }

        [Fact]
        public void Should_return_response_generated_by_route()
        {
            // Arrange
            var expectedResponse = new Response();
            var route = A.Fake<IRoute>();
            var modules = new[] { new FakeNancy() };
            var request = new Request("GET", new Uri("http://localhost"));

            A.CallTo(() => route.Invoke()).Returns(expectedResponse);
            A.CallTo(() => this.locator.GetModules()).Returns(modules);
            A.CallTo(() => this.resolver.GetRoute(request, modules)).Returns(route);

            // Act
            var response = this.engine.HandleRequest(request);

            // Assert
            response.ShouldBeSameAs(expectedResponse);
        }

        private static IRequest ManufactureGETRequest(string route)
        {
            var url =
                string.Concat("http://localhost", route);

            return new Request("GET", new Uri(url));
        }
    }
}