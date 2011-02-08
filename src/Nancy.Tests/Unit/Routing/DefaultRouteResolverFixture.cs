namespace Nancy.Tests.Unit.Routing
{
    using System;
    using System.Net;
    using FakeItEasy;
    using Nancy.Routing;
    using Nancy.Tests.Fakes;
    using Xunit;

    public class RouteResolverFixture
    {
        private readonly DefaultRouteResolver resolver;
        private readonly IRoutePatternMatcher matcher;
        private readonly INancyModuleCatalog catalog;
        private readonly Func<dynamic, Response> expectedAction;
        private readonly ITemplateEngineSelector templateEngineSelector;
        private FakeNancyModule expectedModule;

        public RouteResolverFixture()
        {
            this.templateEngineSelector = A.Fake<ITemplateEngineSelector>();

            this.catalog = A.Fake<INancyModuleCatalog>();
            A.CallTo(() => this.catalog.GetModuleByKey(A<string>.Ignored)).Returns(new FakeNancyModule());

            this.expectedAction = x => HttpStatusCode.OK;
            this.expectedModule = new FakeNancyModule(x =>
            {
                x.AddGetRoute("/foo/bar", this.expectedAction);
            });

            A.CallTo(() => this.catalog.GetModuleByKey(A<string>.Ignored)).ReturnsLazily(() => this.expectedModule);

            this.matcher = A.Fake<IRoutePatternMatcher>();
            A.CallTo(() => this.matcher.Match(A<string>.Ignored, A<string>.Ignored)).ReturnsLazily(x =>
                new FakeRoutePatternMatchResult(c =>
                {
                    c.IsMatch(((string)x.Arguments[0]).Equals(((string)x.Arguments[1])));
                    c.AddParameter("foo", "bar");
                }));

            this.resolver = new DefaultRouteResolver(this.catalog, this.matcher, this.templateEngineSelector);
        }

        [Fact]
        public void Should_set_parameters_on_resolved_route_to_parameteres_that_was_matched()
        {
            // Given
            var request = new FakeRequest("get", "/foo/bar");
            var routeCache = new FakeRouteCache(x =>
            {
                x.AddGetRoute("/foo/bar", "module-key");
            });

            // When
            var resolvedRoute = this.resolver.Resolve(request, routeCache);

            // Then
            resolvedRoute.ShouldNotBeOfType<NotFoundRoute>();
            resolvedRoute.ShouldNotBeOfType<MethodNotAllowedRoute>();
            ((string)resolvedRoute.Parameters["foo"]).ShouldEqual("bar");
        }

        [Fact]
        public void Should_set_action_on_resolved_route()
        {
            // Given
            var request = new FakeRequest("get", "/foo/bar");
            var routeCache = new FakeRouteCache(x =>
            {
                x.AddGetRoute("/foo/bar", "module-key");
            });

            // When
            var resolvedRoute = this.resolver.Resolve(request, routeCache);

            // Then
            resolvedRoute.ShouldNotBeOfType<NotFoundRoute>();
            resolvedRoute.ShouldNotBeOfType<MethodNotAllowedRoute>();
            resolvedRoute.Action.ShouldBeSameAs(expectedAction);
        }

        [Fact]
        public void Should_return_first_route_with_when_multiple_matches_are_available_and_contains_same_number_of_parameter_captures()
        {
            // Given
            var request = new FakeRequest("get", "/foo/bar");
            var routeCache = new FakeRouteCache(x =>
            {
                x.AddGetRoute("/foo/{bar}", "first-module-key-parameters");
                x.AddGetRoute("/foo/{bar}", "second-module-key-parameters");
            });

            this.expectedModule = new FakeNancyModule(x =>
            {
                x.AddGetRoute("/foo/{bar}", this.expectedAction);
            });

            A.CallTo(() => this.matcher.Match(request.Uri, "/foo/{bar}")).Returns(
                new FakeRoutePatternMatchResult(x => x.IsMatch(true).AddParameter("bar", "fake value")));

            // When
            this.resolver.Resolve(request, routeCache);

            // Then
            A.CallTo(() => this.catalog.GetModuleByKey("first-module-key-parameters")).MustHaveHappened();
        }

        [Fact]
        public void Should_return_route_with_most_parameter_captures_when_multiple_matches_are_available()
        {
            // Given
            var request = new FakeRequest("get", "/foo/bar");
            var routeCache = new FakeRouteCache(x =>
            {
                x.AddGetRoute("/foo/bar", "module-key-noparameters");
                x.AddGetRoute("/foo/{bar}", "module-key-parameters");
            });

            this.expectedModule = new FakeNancyModule(x =>
            {
                x.AddGetRoute("/foo/{bar}", this.expectedAction);
            });

            A.CallTo(() => this.matcher.Match(request.Uri, "/foo/bar")).Returns(
                new FakeRoutePatternMatchResult(x => x.IsMatch(true)));

            A.CallTo(() => this.matcher.Match(request.Uri, "/foo/{bar}")).Returns(
                new FakeRoutePatternMatchResult(x => x.IsMatch(true).AddParameter("bar", "fake value")));

            // When
            this.resolver.Resolve(request, routeCache);

            // Then
            A.CallTo(() => this.catalog.GetModuleByKey("module-key-parameters")).MustHaveHappened();
        }

        [Fact]
        public void Should_invoke_pattern_matcher_with_request_uri()
        {
            // Given
            var request = new FakeRequest("get", "/foo/bar");
            var routeCache = new FakeRouteCache(x =>
            {
                x.AddGetRoute("/foo/bar");
            });

            // When
            this.resolver.Resolve(request, routeCache);

            // Then
            A.CallTo(() => this.matcher.Match(request.Uri, A<string>.Ignored)).MustHaveHappened();
        }

        [Fact]
        public void Should_invoke_pattern_matcher_for_all_entries_in_route_cache()
        {
            // Given
            var request = new FakeRequest("get", "/foo/bar");
            var routeCache = new FakeRouteCache(x =>
            {
                x.AddGetRoute("/foo/bar");
                x.AddGetRoute("/bar/foo");
                x.AddGetRoute("/foobar");
            });

            // When
            this.resolver.Resolve(request, routeCache);

            // Then
            A.CallTo(() => this.matcher.Match(A<string>.Ignored, "/foo/bar")).MustHaveHappened();
            A.CallTo(() => this.matcher.Match(A<string>.Ignored, "/bar/foo")).MustHaveHappened();
            A.CallTo(() => this.matcher.Match(A<string>.Ignored, "/foobar")).MustHaveHappened();
        }

        [Fact]
        public void Should_ignore_method_casing_when_resolving_routes()
        {
            var request = new FakeRequest("GeT", "/foo/bar");
            var routeCache = new FakeRouteCache(x =>
            {
                x.AddGetRoute("/foo/bar");
            });

            // When
            var resolvedRoute = this.resolver.Resolve(request, routeCache);

            // Then
            resolvedRoute.ShouldNotBeNull();
            resolvedRoute.ShouldNotBeOfType<NotFoundRoute>();
            resolvedRoute.ShouldNotBeOfType<MethodNotAllowedRoute>();
        }

        [Fact]
        public void Should_return_methodnotallowedroute_with_path_set_to_request_uri_when_matched_route_was_for_wrong_request_method()
        {
            // Given
            var request = new FakeRequest("POST", "/foo/bar");
            var routeCache = new FakeRouteCache(x =>
            {
                x.AddGetRoute("/foo/bar");
            });

            // When
            var route = this.resolver.Resolve(request, routeCache);

            // Then
            route.ShouldNotBeNull();
            route.ShouldBeOfType<MethodNotAllowedRoute>();
            route.Path.ShouldEqual(request.Uri);
        }

        [Fact]
        public void Should_set_template_engine_selector_on_module_associated_with_resolved_route()
        {
            // Given
            var request = new FakeRequest("GET", "/foo/bar");
            var routeCache = new FakeRouteCache(x =>
            {
                x.AddGetRoute("/foo/bar", "module-key");
            });

            // When
            var resolvedRoute = this.resolver.Resolve(request, routeCache);

            // Then
            resolvedRoute.Module.TemplateEngineSelector.ShouldBeSameAs(this.templateEngineSelector);
        }

        [Fact]
        public void Should_set_request_on_module_associated_with_resolved_route()
        {
            // Given
            var request = new FakeRequest("GET", "/foo/bar");
            var routeCache = new FakeRouteCache(x =>
            {
                x.AddGetRoute("/foo/bar", "module-key");
            });

            // When
            var resolvedRoute = this.resolver.Resolve(request, routeCache);

            // Then
            resolvedRoute.Module.Request.ShouldBeSameAs(request);
        }

        [Fact]
        public void Should_return_route_with_module_set_to_module_associated_with_resolved_route()
        {
            // Given
            var request = new FakeRequest("GET", "/foo/bar");
            var routeCache = new FakeRouteCache(x =>
            {
                x.AddGetRoute("/foo/bar", "module-key");
            });

            // When
            var resolvedRoute = this.resolver.Resolve(request, routeCache);

            // Then
            resolvedRoute.ShouldNotBeNull();
            resolvedRoute.ShouldNotBeOfType<NotFoundRoute>();
            resolvedRoute.Module.ShouldBeSameAs(expectedModule);
        }

        [Fact]
        public void Should_return_route_with_path_set_to_request_uri_when_single_route_could_be_resolved()
        {
            // Given
            var request = new FakeRequest("GET", "/foo/bar");
            var routeCache = new FakeRouteCache(x =>
            {
                x.AddGetRoute("/foo/bar");
            });

            // When
            var resolvedRoute = this.resolver.Resolve(request, routeCache);

            // Then
            resolvedRoute.ShouldNotBeOfType<NotFoundRoute>();
            resolvedRoute.Path.ShouldEqual(request.Uri);
        }

        [Fact]
        public void Should_return_notfoundroute_with_path_set_to_request_uri_when_route_cache_is_empty()
        {
            // Given
            var request = new FakeRequest("GET", "/foo/bar");
            var routeCache = FakeRouteCache.Empty;

            // When
            var resolvedRoute = this.resolver.Resolve(request, routeCache);

            // Then
            resolvedRoute.ShouldBeOfType<NotFoundRoute>();
            resolvedRoute.Path.ShouldEqual(request.Uri);
        }

        [Fact]
        public void Should_return_notfoundroute_with_path_set_to_request_uri_when_route_could_not_be_resolved()
        {
            // Given
            var request = new FakeRequest("GET", "/foo/bar");
            var routeCache = new FakeRouteCache(x =>
            {
                x.AddGetRoute("/bar/foo");
            });

            A.CallTo(() => this.matcher.Match("/foo/bar", "/bar/foo")).Returns(
                new FakeRoutePatternMatchResult(x => x.IsMatch(false)));

            // When
            var resolvedRoute = this.resolver.Resolve(request, routeCache);

            // Then
            resolvedRoute.ShouldBeOfType<NotFoundRoute>();
            resolvedRoute.Path.ShouldEqual(request.Uri);
        }

        [Fact]
        public void Should_allow_head_request_when_route_is_defined_for_get()
        {
            var request = new FakeRequest("HEAD", "/foo/bar");
            var routeCache = new FakeRouteCache(x =>
            {
                x.AddGetRoute("/foo/bar");
            });

            var resolvedRoute = this.resolver.Resolve(request, routeCache);
            
            resolvedRoute.ShouldNotBeNull();
            resolvedRoute.ShouldNotBeOfType<NotFoundRoute>();
            resolvedRoute.ShouldNotBeOfType<MethodNotAllowedRoute>();
        }
    }
}