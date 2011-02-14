namespace Nancy.Tests.Unit
{
    using System;
    using System.Linq;

    using FakeItEasy;    
    using Nancy.Routing;
    using Nancy.Tests.Fakes;
    using Nancy.ViewEngines;

    using Xunit;

    public class NancyEngineFixture
    {
        private readonly INancyEngine engine;
        private readonly IRouteResolver resolver;
        private readonly FakeRoute route;

        public NancyEngineFixture()
        {
            this.resolver = A.Fake<IRouteResolver>();
            this.route = new FakeRoute();

            A.CallTo(() => resolver.Resolve(A<Request>.Ignored, A<IRouteCache>.Ignored.Argument)).Returns(new Tuple<Route, DynamicDictionary>(route, new DynamicDictionary()));
            this.engine = new NancyEngine(resolver, A.Fake<IRouteCache>());
        }

        [Fact]
        public void Should_throw_argumentnullexception_when_created_with_null_resolver()
        {
            // Given, When
            var exception =
                Record.Exception(() => new NancyEngine(null, A.Fake<IRouteCache>()));

            // Then
            exception.ShouldBeOfType<ArgumentNullException>();
        }

        [Fact]
        public void Should_throw_argumentnullexception_when_created_with_null_routecache()
        {
            // Given, When
            var exception =
                Record.Exception(() => new NancyEngine(A.Fake<IRouteResolver>(), null));

            // Then
            exception.ShouldBeOfType<ArgumentNullException>();
        }

        [Fact]
        public void Should_invoke_resolved_route()
        {
            // Given
            var request = new Request("GET", "/", "http");

            // When
            this.engine.HandleRequest(request);

            // Then
            this.route.ActionWasInvoked.ShouldBeTrue();
        }

        [Fact]
        public void HandleRequest_Should_Throw_ArgumentNullException_When_Given_A_Null_Request()
        {
            var exception = Record.Exception(() => engine.HandleRequest(null));

            // Then
            exception.ShouldBeOfType<ArgumentNullException>();
        }
        //[Fact]
        //public void Should_treat_a_HEAD_request_like_a_GET_when_getting_a_request_to_route_resolver()
        //{
        //    // Given
        //    var request = new Request("HEAD", "/", "http");


        //    A.CallTo(() => this.locator.GetModules()).Returns(modules);

        //    // When
        //    this.engine.HandleRequest(request);

        //    // Then
        //    A.CallTo(() => this.resolver.GetRoute(A<Request>.Ignored.Argument,
        //        A<IEnumerable<ModuleMeta>>.That.Matches(x => x.SequenceEqual(this.modules["GET"])).Argument, A<ITemplateEngineSelector>.Ignored.Argument)).MustHaveHappened();
        //}
    }
}
