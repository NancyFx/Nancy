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
        private readonly INancyModuleLocator locator;
        private readonly INancyApplication application;
        private readonly IDictionary<string, IEnumerable<ModuleMeta>> modules;

        public NancyEngineFixture()
        {
            this.modules = new NancyApplication(new DefaultModuleActivator()).GetModules();
            this.locator = A.Fake<INancyModuleLocator>();
            this.resolver = A.Fake<IRouteResolver>();
            this.application = A.Fake<INancyApplication>();
            this.engine = new NancyEngine(this.locator, this.resolver, this.application);
        }

        [Fact]
        public void Should_throw_argumentnullexception_when_created_with_null_locator()
        {
            // Given, When
            var exception =
                Record.Exception(() => new NancyEngine(null, A.Fake<IRouteResolver>(), A.Fake<INancyApplication>()));

            // Then
            exception.ShouldBeOfType<ArgumentNullException>();
        }

        [Fact]
        public void Should_throw_argumentnullexception_when_created_with_null_resolver()
        {
            // Given, When
            var exception =
                Record.Exception(() => new NancyEngine(A.Fake<INancyModuleLocator>(), null, A.Fake<INancyApplication>()));

            // Then
            exception.ShouldBeOfType<ArgumentNullException>();
        }

        [Fact]
        public void Should_throw_argumentnullexception_when_created_with_null_application()
        {
            // Given, When
            var exception =
                Record.Exception(() => new NancyEngine(A.Fake<INancyModuleLocator>(),  A.Fake<IRouteResolver>(), null));

            // Then
            exception.ShouldBeOfType<ArgumentNullException>();
        }

        [Fact]
        public void Should_retrieve_modules_from_locator_when_handling_request()
        {
            // Given
            var request = new Request("GET", "/");

            // When
            this.engine.HandleRequest(request);

            // Then
            A.CallTo(() => this.locator.GetModules()).MustHaveHappened();
        }

        [Fact]
        public void Should_return_not_found_response_when_no_nancy_modules_could_be_found()
        {
            // Given
            var request = new Request("GET", "/");

            A.CallTo(() => this.locator.GetModules()).Returns(new Dictionary<string, IEnumerable<ModuleMeta>>());

            // When
            var response = this.engine.HandleRequest(request);

            // Then
            response.StatusCode.ShouldEqual(HttpStatusCode.NotFound);
        }

        [Fact]
        public void Should_pass_all_registered_route_handlers_for_get_request_to_route_resolver()
        {
            // Given
            var request = new Request("GET", "/");
            

            A.CallTo(() => this.locator.GetModules()).Returns(modules);

            // When
            this.engine.HandleRequest(request);

            // Then
            A.CallTo(() => this.resolver.GetRoute(A<Request>.Ignored.Argument, 
                A<IEnumerable<ModuleMeta>>.That.Matches(x => x.SequenceEqual(this.modules["GET"])).Argument, 
                A<INancyApplication>.Ignored.Argument)).MustHaveHappened();
        }

        [Theory]
        [InlineData("get")]
        [InlineData("GeT")]
        [InlineData("PoSt")]
        [InlineData("post")]
        [InlineData("puT")]
        [InlineData("PUT")]
        [InlineData("DelETe")]
        [InlineData("DELete")]
        public void Should_ignore_case_of_request_verb_when_resolving_route_handlers(string verb)
        {
            // Given
            var request = new Request(verb, "/");
            
            A.CallTo(() => this.locator.GetModules()).Returns(this.modules);

            // When
            this.engine.HandleRequest(request);

            // Then
            A.CallTo(() => this.resolver.GetRoute(A<Request>.Ignored.Argument,
                A<IEnumerable<ModuleMeta>>.Ignored.Argument, A<INancyApplication>.Ignored.Argument)).MustHaveHappened();
        }

        [Fact]
        public void Should_treat_a_HEAD_request_like_a_GET_when_getting_a_request_to_route_resolver()
        {
            // Given
            var request = new Request("HEAD", "/");


            A.CallTo(() => this.locator.GetModules()).Returns(modules);

            // When
            this.engine.HandleRequest(request);

            // Then
            A.CallTo(() => this.resolver.GetRoute(A<Request>.Ignored.Argument,
                A<IEnumerable<ModuleMeta>>.That.Matches(x => x.SequenceEqual(this.modules["GET"])).Argument, A<INancyApplication>.Ignored.Argument)).MustHaveHappened();
        }

        [Fact]
        public void Should_pass_all_registered_route_handlers_for_delete_request_to_route_resolver()
        {
            // Given
            var request = new Request("DELETE", "/");
            
            A.CallTo(() => this.locator.GetModules()).Returns(this.modules);

            // When
            this.engine.HandleRequest(request);

            // Then
            A.CallTo(() => this.resolver.GetRoute(A<Request>.Ignored.Argument, A<IEnumerable<ModuleMeta>>.That.Matches(x => x.SequenceEqual(this.modules["DELETE"])).Argument, A<INancyApplication>.Ignored.Argument)).MustHaveHappened();
        }

        [Fact]
        public void Should_call_route_resolver_with_all_route_handlers()
        {
            // Given
            var request = new Request("PUT", "/");
            
            A.CallTo(() => this.locator.GetModules()).Returns(this.modules);

            // When
            this.engine.HandleRequest(request);

            // Then
            A.CallTo(() => this.resolver.GetRoute(A<Request>.Ignored.Argument, A<IEnumerable<ModuleMeta>>.That.Matches(x => x.SequenceEqual(this.modules["PUT"])).Argument, A<INancyApplication>.Ignored.Argument)).MustHaveHappened();
        }

        [Fact]
        public void Should_call_route_resolver_with_request()
        {
            // Given
            var request = new Request("GET", "/");

            A.CallTo(() => this.locator.GetModules()).Returns(this.modules);

            // When
            this.engine.HandleRequest(request);

            // Then
            A.CallTo(() => this.resolver.GetRoute(request,
                A<IEnumerable<ModuleMeta>>.Ignored.Argument, A<INancyApplication>.Ignored.Argument)).MustHaveHappened();
        }

        [Fact]
        public void Should_return_not_found_response_when_no_route_could_be_matched_for_the_request_verb()
        {
            // Given
            var request = new Request("NOTVALID", "/");

            A.CallTo(() => this.locator.GetModules()).Returns(this.modules);

            // When
            var response = this.engine.HandleRequest(request);

            // Then
            response.StatusCode.ShouldEqual(HttpStatusCode.NotFound);
        }

        [Fact]
        public void Should_throw_argumentnullexception_when_handling_null_request()
        {
            // Given, When
            var exception =
                Record.Exception(() => this.engine.HandleRequest(null));

            // Then
            exception.ShouldBeOfType<ArgumentNullException>();
        }

        [Fact]
        public void Should_invoke_resolved_route()
        {
            // Given
            var route = A.Fake<IRoute>();
            var request = new Request("GET", "/");            

            A.CallTo(() => this.locator.GetModules()).Returns(this.modules);
            A.CallTo(() => this.resolver.GetRoute(request, A<IEnumerable<ModuleMeta>>.Ignored.Argument, A<INancyApplication>.Ignored.Argument)).Returns(route);

            // When
            this.engine.HandleRequest(request);

            // Then
            A.CallTo(() => route.Invoke()).MustHaveHappened();
        }

        [Fact]
        public void Should_return_response_generated_by_route()
        {
            // Given
            var expectedResponse = new Response();
            var route = A.Fake<IRoute>();
            var request = new Request("GET", "/");
            var descriptions = GetRouteDescriptions(request, this.modules);

            A.CallTo(() => route.Invoke()).Returns(expectedResponse);
            A.CallTo(() => this.locator.GetModules()).Returns(this.modules);
            A.CallTo(() => this.resolver.GetRoute(request, A<IEnumerable<ModuleMeta>>.Ignored.Argument, A<INancyApplication>.Ignored.Argument)).Returns(route);

            // When
            var response = this.engine.HandleRequest(request);

            // Then
            response.ShouldBeSameAs(expectedResponse);
        }

        [Fact]
        public void Should_set_base_route_on_descriptions_that_are_passed_to_resolver()
        {
            // Given
            var request = new Request("POST", "/fake/");

            var r = new FakeRouteResolver();
            var e = new NancyEngine(this.locator, r, this.application);

            A.CallTo(() => this.locator.GetModules()).Returns(this.modules);

            // When
            e.HandleRequest(request);

            // Then
            r.ModulePath.ShouldEqual("/fake");
        }

        [Fact]
        public void Should_set_path_on_descriptions_that_are_passed_to_resolver()
        {
            // Given
            var request = new Request("POST", "/");

            var r = new FakeRouteResolver();
            var e = new NancyEngine(this.locator, r, this.application);

            A.CallTo(() => this.locator.GetModules()).Returns(this.modules);

            // When
            e.HandleRequest(request);

            // Then
            r.Path.ShouldEqual("/");
        }
        
        private static IEnumerable<RouteDescription> GetRouteDescriptions(IRequest request, IDictionary<string, IEnumerable<ModuleMeta>> modules)
        {
            return modules.First().Value.SelectMany(s => s.RouteDescriptions);
        }
    }
}
