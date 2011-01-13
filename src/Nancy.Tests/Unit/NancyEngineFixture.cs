namespace Nancy.Tests.Unit
{
    using System;
    using System.Collections.Generic;    
    using System.Linq;
    using System.Net;
    using FakeItEasy;    
    using Nancy.Routing;
    using Nancy.Tests.Fakes;
    using Xunit;
    using Xunit.Extensions;

    public class NancyEngineFixture
    {
        private readonly INancyEngine engine;
        private readonly IRouteResolver resolver;
        private readonly IRoute route;

        public NancyEngineFixture()
        {
            this.resolver = A.Fake<IRouteResolver>();
            this.route = A.Fake<IRoute>();

            A.CallTo(() => resolver.GetRoute(A<IRequest>.Ignored.Argument)).Returns(route);
            this.engine = new NancyEngine(resolver);
        }

        [Fact]
        public void Should_throw_argumentnullexception_when_created_with_null_resolver()
        {
            // Given, When
            var exception =
                Record.Exception(() => new NancyEngine(null));

            // Then
            exception.ShouldBeOfType<ArgumentNullException>();
        }

        [Fact]
        public void Should_invoke_route_returned()
        {
            // Given
            var request = new Request("GET", "/", "http");

            // When
            this.engine.HandleRequest(request);

            // Then
            A.CallTo(() => this.route.Invoke()).MustHaveHappened();
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
