namespace Nancy.Tests.Unit.Routing
{
    using System;
    using FakeItEasy;
    using Nancy.Routing;
    using Nancy.Tests.Fakes;

    using Xunit;

    public class DefaultRouteResolverFixture
    {
        private readonly DefaultRouteResolver resolver;
        private readonly IRoutePatternMatcher matcher;
        private readonly INancyModuleCatalog catalog;
        private readonly Func<dynamic, Response> expectedAction;
        private readonly INancyModuleBuilder moduleBuilder;
        private FakeNancyModule expectedModule;

        public DefaultRouteResolverFixture()
        {
            this.moduleBuilder = A.Fake<INancyModuleBuilder>();
            A.CallTo(() => this.moduleBuilder.BuildModule(A<NancyModule>.Ignored, A<NancyContext>.Ignored)).
                ReturnsLazily(r => r.Arguments[0] as NancyModule);

            this.catalog = A.Fake<INancyModuleCatalog>();
            A.CallTo(() => this.catalog.GetModuleByKey(A<string>.Ignored, A<NancyContext>.Ignored)).Returns(expectedModule);

            this.expectedAction = x => HttpStatusCode.OK;
            this.expectedModule = new FakeNancyModule(x =>
            {
                x.AddGetRoute("/foo/bar", this.expectedAction);
            });

            A.CallTo(() => this.catalog.GetModuleByKey(A<string>.Ignored, A<NancyContext>.Ignored)).ReturnsLazily(() => this.expectedModule);

            this.matcher = A.Fake<IRoutePatternMatcher>();
            A.CallTo(() => this.matcher.Match(A<string>.Ignored, A<string>.Ignored)).ReturnsLazily(x =>
                new FakeRoutePatternMatchResult(c =>
                {
                    c.IsMatch(((string)x.Arguments[0]).Equals(((string)x.Arguments[1])));
                    c.AddParameter("foo", "bar");
                }));

            this.resolver = new DefaultRouteResolver(this.catalog, this.matcher, this.moduleBuilder);
        }

        [Fact]
        public void Should_set_parameters_on_resolved_route_to_parameteres_that_was_matched()
        {
            // Given
            var request = new FakeRequest("get", "/foo/bar");
            var context = new NancyContext();
            context.Request = request;
            var routeCache = new FakeRouteCache(x =>
            {
                x.AddGetRoute("/foo/bar", "module-key");
            });

            // When
            var resolvedRoute = this.resolver.Resolve(context, routeCache);

            // Then
            resolvedRoute.ShouldNotBeOfType<NotFoundRoute>();
            resolvedRoute.ShouldNotBeOfType<MethodNotAllowedRoute>();
            ((string)resolvedRoute.Item2["foo"]).ShouldEqual("bar");
        }

        [Fact]
        public void Should_set_action_on_resolved_route()
        {
            // Given
            var request = new FakeRequest("get", "/foo/bar");
            var context = new NancyContext();
            context.Request = request;
            var routeCache = new FakeRouteCache(x =>
            {
                x.AddGetRoute("/foo/bar", "module-key");
            });

            // When
            var resolvedRoute = this.resolver.Resolve(context, routeCache).Item1;

            // Then
            resolvedRoute.ShouldNotBeOfType<NotFoundRoute>();
            resolvedRoute.ShouldNotBeOfType<MethodNotAllowedRoute>();
            resolvedRoute.Action.ShouldBeSameAs(this.expectedAction);
        }

        [Fact]
        public void Should_return_first_route_with_when_multiple_matches_are_available_and_contains_same_number_of_parameter_captures()
        {
            // Given
            var request = new FakeRequest("get", "/foo/bar");
            var context = new NancyContext();
            context.Request = request;
            var routeCache = new FakeRouteCache(x =>
            {
                x.AddGetRoute("/foo/{bar}", "first-module-key-parameters");
                x.AddGetRoute("/foo/{bar}", "second-module-key-parameters");
            });

            this.expectedModule = new FakeNancyModule(x =>
            {
                x.AddGetRoute("/foo/{bar}", this.expectedAction);
            });

            A.CallTo(() => this.matcher.Match(request.Path, "/foo/{bar}")).Returns(
                new FakeRoutePatternMatchResult(x => x.IsMatch(true).AddParameter("bar", "fake value")));

            // When
            this.resolver.Resolve(context, routeCache);

            // Then
            A.CallTo(() => this.catalog.GetModuleByKey("first-module-key-parameters", context)).MustHaveHappened();
        }

        [Fact]
        public void Should_return_route_with_most_parameter_captures_when_multiple_matches_with_parameters_are_available()
        {
            // Given
            var request = new FakeRequest("get", "/foo/bar/foo");
            var context = new NancyContext { Request = request };
            var routeCache = new FakeRouteCache(x =>
            {
                x.AddGetRoute("/foo/{bar}/{foo}", "module-key-two-parameters");
                x.AddGetRoute("/foo/{bar}", "module-key-one-parameter");
            });

            A.CallTo(() => this.matcher.Match(request.Path, "/foo/{bar}")).Returns(
                new FakeRoutePatternMatchResult(x => x.IsMatch(true).AddParameter("bar", "fake value")));

            A.CallTo(() => this.matcher.Match(request.Path, "/foo/{bar}/{foo}")).Returns(
                new FakeRoutePatternMatchResult(x => x.IsMatch(true)
                    .AddParameter("foo", "fake value")
                    .AddParameter("bar", "fake value 2")));

            // When
            this.resolver.Resolve(context, routeCache);

            // Then
            A.CallTo(() => this.catalog.GetModuleByKey("module-key-two-parameters", context)).MustHaveHappened();
        }

        [Fact]
        public void Should_return_the_first_route_that_is_an_exact_match_over_any_other()
        {
            // Given
            var request = new FakeRequest("get", "/foo/bar");
            var context = new NancyContext { Request = request };
            var routeCache = new FakeRouteCache(x =>
            {
                x.AddGetRoute("/foo/{bar}", "module-key-parameters");
                x.AddGetRoute("/{foo}/{bar}", "module-key-two-parameters");
                x.AddGetRoute("/foo/bar", "module-key-no-parameters");
                x.AddGetRoute("/foo/bar", "module-key-no-parameters-second");
                x.AddGetRoute("/foo/{bar}", "module-key-parameters");
                x.AddGetRoute("/{foo}/{bar}", "module-key-two-parameters");
            });

            A.CallTo(() => this.matcher.Match(request.Path, "/foo/bar")).Returns(
                new FakeRoutePatternMatchResult(x => x.IsMatch(true)));

            A.CallTo(() => this.matcher.Match(request.Path, "/foo/{bar}")).Returns(
                new FakeRoutePatternMatchResult(x => x.IsMatch(true).AddParameter("bar", "fake value")));

            A.CallTo(() => this.matcher.Match(request.Path, "/foo/{bar}")).Returns(
                new FakeRoutePatternMatchResult(x => x.IsMatch(true)
                    .AddParameter("foo", "fake value")
                    .AddParameter("bar", "fake value")));
            // When
            this.resolver.Resolve(context, routeCache);

            // Then
            A.CallTo(() => this.catalog.GetModuleByKey("module-key-no-parameters", context)).MustHaveHappened();
        }

        [Fact]
        public void Should_return_the_route_with_the_most_specific_path_matches()
        {
            // The most specific path match is the one with the most matching segments (delimited by '/') and the
            // least parameter captures (i.e. the most exact matching segments)
            // Given
            var request = new FakeRequest("get", "/foo/bar/me");
            var context = new NancyContext { Request = request };
            var routeCache = new FakeRouteCache(x =>
            {
                x.AddGetRoute("/foo/{bar}", "module-key-first");
                x.AddGetRoute("/foo/{bar}/{two}", "module-key-third");
                x.AddGetRoute("/foo/bar/{two}", "module-key-second");
                x.AddGetRoute("/foo/{bar}/{two}", "module-key-third");
            });

            this.expectedModule = new FakeNancyModule(x => x.AddGetRoute("/foo/bar/{two}", this.expectedAction));

            A.CallTo(() => this.matcher.Match(request.Path, "/foo/bar/{two}")).Returns(
                new FakeRoutePatternMatchResult(x => x.IsMatch(true).AddParameter("two", "fake values")));

            A.CallTo(() => this.matcher.Match(request.Path, "/foo/{bar}/{two}")).Returns(
                new FakeRoutePatternMatchResult(x => x.IsMatch(true)
                    .AddParameter("bar", "fake values")
                    .AddParameter("two", "fake values")));

            A.CallTo(() => this.matcher.Match(request.Path, "/foo/{bar}")).Returns(
                new FakeRoutePatternMatchResult(x => x.IsMatch(true).AddParameter("bar", "fake value")));

            // When
            this.resolver.Resolve(context, routeCache);

            // Then
            A.CallTo(() => this.catalog.GetModuleByKey("module-key-second", context)).MustHaveHappened();
        }

        [Fact]
        public void Should_choose_root_route_over_one_with_capture_if_requesting_root_uri()
        {
            var request = new FakeRequest("get", "/");
            var context = new NancyContext { Request = request };
            var routeCache = new FakeRouteCache(x =>
            {
                x.AddGetRoute("/{name}", "module-key-second");
                x.AddGetRoute("/", "module-key-first");
                x.AddGetRoute("/{name}", "module-key-second");
            });
            A.CallTo(() => this.matcher.Match(request.Path, "/")).Returns(
                new FakeRoutePatternMatchResult(x => x.IsMatch(true)));
            A.CallTo(() => this.matcher.Match(request.Path, "/{name}")).Returns(
                new FakeRoutePatternMatchResult(x => x.IsMatch(true).AddParameter("name", "fake values")));

            // When
            this.resolver.Resolve(context, routeCache);

            // Then
            A.CallTo(() => this.catalog.GetModuleByKey("module-key-first", context)).MustHaveHappened();
        }

        [Fact]
        public void Should_invoke_pattern_matcher_with_request_uri()
        {
            // Given
            var request = new FakeRequest("get", "/foo/bar");
            var context = new NancyContext();
            context.Request = request;
            var routeCache = new FakeRouteCache(x =>
            {
                x.AddGetRoute("/foo/bar");
            });

            // When
            this.resolver.Resolve(context, routeCache);

            // Then
            A.CallTo(() => this.matcher.Match(request.Path, A<string>.Ignored)).MustHaveHappened();
        }

        [Fact]
        public void Should_invoke_pattern_matcher_for_all_entries_in_route_cache()
        {
            // Given
            var request = new FakeRequest("get", "/foo/bar");
            var context = new NancyContext();
            context.Request = request;
            var routeCache = new FakeRouteCache(x =>
            {
                x.AddGetRoute("/foo/bar");
                x.AddGetRoute("/bar/foo");
                x.AddGetRoute("/foobar");
            });

            // When
            this.resolver.Resolve(context, routeCache);

            // Then
            A.CallTo(() => this.matcher.Match(A<string>.Ignored, "/foo/bar")).MustHaveHappened();
            A.CallTo(() => this.matcher.Match(A<string>.Ignored, "/bar/foo")).MustHaveHappened();
            A.CallTo(() => this.matcher.Match(A<string>.Ignored, "/foobar")).MustHaveHappened();
        }

        [Fact]
        public void Should_ignore_method_casing_when_resolving_routes()
        {
            var request = new FakeRequest("GeT", "/foo/bar");
            var context = new NancyContext();
            context.Request = request;
            var routeCache = new FakeRouteCache(x =>
            {
                x.AddGetRoute("/foo/bar");
            });

            // When
            var resolvedRoute = this.resolver.Resolve(context, routeCache);

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
            var context = new NancyContext();
            context.Request = request;
            var routeCache = new FakeRouteCache(x =>
            {
                x.AddGetRoute("/foo/bar");
            });

            // When
            var route = this.resolver.Resolve(context, routeCache).Item1;

            // Then
            route.ShouldNotBeNull();
            route.ShouldBeOfType<MethodNotAllowedRoute>();
            route.Description.Path.ShouldEqual(request.Path);
        }

        [Fact]
        public void Should_return_methodnotallowedroute_with_allow_header_set_to_allowed_methods_matching_request_route()
        {
            // Given
            var request = new FakeRequest("POST", "/foo/bar");
            var context = new NancyContext();
            context.Request = request;
            var routeCache = new FakeRouteCache(x =>
            {
                x.AddGetRoute("/foo/bar");
                x.AddPutRoute("/foo/bar");
            });

            // When
            var route = this.resolver.Resolve(context, routeCache).Item1;

            // Then
            route.ShouldNotBeNull();
            route.ShouldBeOfType<MethodNotAllowedRoute>();
            route.Invoke(new DynamicDictionary()).Headers["Allow"].ShouldEqual("GET, PUT");
        }

        [Fact]
        public void Should_invoke_module_builder_with_context_and_resolved_module()
        {
            // Given
            var request = new FakeRequest("GET", "/foo/bar");
            var context = new NancyContext { Request = request };
            var routeCache = new FakeRouteCache(x => x.AddGetRoute("/foo/bar"));

            // When
            this.resolver.Resolve(context, routeCache);

            // Then
            A.CallTo(() => this.moduleBuilder.BuildModule(this.expectedModule, context)).MustHaveHappened();
        }

        [Fact]
        public void Should_return_route_with_module_returned_by_module_builder()
        {
            // Given
            var request = new FakeRequest("GET", "/foo/bar");
            var context = new NancyContext { Request = request };
            var routeCache = new FakeRouteCache(x => x.AddGetRoute("/foo/bar"));

            var moduleReturnedByBuilder = new FakeNancyModule(x => x.AddGetRoute("/bar/foo"));
            A.CallTo(() => this.moduleBuilder.BuildModule(A<NancyModule>.Ignored, A<NancyContext>.Ignored)).Returns(
                moduleReturnedByBuilder);

            // When
            var route = this.resolver.Resolve(context, routeCache);

            // Then
            route.Item1.Description.Path.ShouldEqual("/bar/foo");
        }

        [Fact]
        public void Should_return_route_with_path_set_to_request_uri_when_single_route_could_be_resolved()
        {
            // Given
            var request = new FakeRequest("GET", "/foo/bar");
            var context = new NancyContext();
            context.Request = request;
            var routeCache = new FakeRouteCache(x =>
            {
                x.AddGetRoute("/foo/bar");
            });

            // When
            var resolvedRoute = this.resolver.Resolve(context, routeCache);

            // Then
            resolvedRoute.ShouldNotBeOfType<NotFoundRoute>();
            resolvedRoute.Item1.Description.Path.ShouldEqual(request.Path);
        }

        [Fact]
        public void should_return_notfoundroute_with_path_set_to_request_uri_when_route_cache_is_empty()
        {
            // Given
            var request = new FakeRequest("GET", "/foo/bar");
            var context = new NancyContext();
            context.Request = request;
            var routeCache = FakeRouteCache.Empty;

            // When
            var resolvedRoute = this.resolver.Resolve(context, routeCache).Item1;

            // Then
            resolvedRoute.ShouldBeOfType<NotFoundRoute>();
            resolvedRoute.Description.Path.ShouldEqual(request.Path);
        }

        [Fact]
        public void Should_return_notfoundroute_with_path_set_to_request_uri_when_route_could_not_be_resolved()
        {
            // Given
            var request = new FakeRequest("GET", "/foo/bar");
            var context = new NancyContext();
            context.Request = request;
            var routeCache = new FakeRouteCache(x =>
            {
                x.AddGetRoute("/bar/foo");
            });

            A.CallTo(() => this.matcher.Match("/foo/bar", "/bar/foo")).Returns(
                new FakeRoutePatternMatchResult(x => x.IsMatch(false)));

            // When
            var resolvedRoute = this.resolver.Resolve(context, routeCache).Item1;

            // Then
            resolvedRoute.ShouldBeOfType<NotFoundRoute>();
            resolvedRoute.Description.Path.ShouldEqual(request.Path);
        }

        [Fact]
        public void Should_allow_head_request_when_route_is_defined_for_get()
        {
            // Given
            var request = new FakeRequest("HEAD", "/foo/bar");
            var context = new NancyContext();
            context.Request = request;
            var routeCache = new FakeRouteCache(x =>
            {
                x.AddGetRoute("/foo/bar");
            });

            // When
            var resolvedRoute = this.resolver.Resolve(context, routeCache);

            // Then
            resolvedRoute.ShouldNotBeNull();
            resolvedRoute.ShouldNotBeOfType<NotFoundRoute>();
            resolvedRoute.ShouldNotBeOfType<MethodNotAllowedRoute>();
        }

        [Fact]
        public void Should_not_return_a_route_if_matching_and_the_filter_returns_false()
        {
            // Given
            var moduleCatalog = new FakeModuleCatalog();
            var routeCache = new RouteCache(moduleCatalog, new FakeModuleKeyGenerator(), A.Fake<INancyContextFactory>());
            var specificResolver = new DefaultRouteResolver(moduleCatalog, this.matcher, this.moduleBuilder);
            var request = new FakeRequest("GET", "/filtered");
            var context = new NancyContext { Request = request };

            // When
            var route = specificResolver.Resolve(context, routeCache).Item1;

            // Then
            route.ShouldBeOfType(typeof(NotFoundRoute));
        }

        [Fact]
        public void Should_return_a_route_if_matching_and_the_filter_returns_true()
        {
            // Given
            var moduleCatalog = new FakeModuleCatalog();
            var routeCache = new RouteCache(moduleCatalog, new FakeModuleKeyGenerator(), A.Fake<INancyContextFactory>());
            var specificResolver = new DefaultRouteResolver(moduleCatalog, this.matcher, this.moduleBuilder);
            var request = new FakeRequest("GET", "/notfiltered");
            var context = new NancyContext { Request = request };

            // When
            var route = specificResolver.Resolve(context, routeCache).Item1;

            // Then
            route.ShouldBeOfType(typeof(Route));
        }

        [Fact]
        public void Should_return_route_whos_filter_returns_true_when_there_is_also_a_matching_route_with_a_failing_filter()
        {
            // Given
            var moduleCatalog = new FakeModuleCatalog();
            var routeCache = new RouteCache(moduleCatalog, new FakeModuleKeyGenerator(), A.Fake<INancyContextFactory>());
            var specificResolver = new DefaultRouteResolver(moduleCatalog, this.matcher, this.moduleBuilder);
            var request = new FakeRequest("GET", "/filt");
            var context = new NancyContext { Request = request };

            // When
            var route = specificResolver.Resolve(context, routeCache).Item1;

            // Then
            route.Description.Condition(context).ShouldBeTrue();
        }

        [Fact]
        public void Should_return_prereq_and_postreq_from_module()
        {
            // Given
            var moduleCatalog = A.Fake<INancyModuleCatalog>();
            A.CallTo(() => moduleCatalog.GetAllModules(A<NancyContext>.Ignored)).Returns(new[] { new FakeNancyModuleWithPreAndPostHooks() });
            A.CallTo(() => moduleCatalog.GetModuleByKey(A<string>.Ignored, A<NancyContext>.Ignored)).Returns(
                new FakeNancyModuleWithPreAndPostHooks());

            var routeCache = new RouteCache(moduleCatalog, new FakeModuleKeyGenerator(), A.Fake<INancyContextFactory>());
            var specificResolver = new DefaultRouteResolver(moduleCatalog, this.matcher, this.moduleBuilder);
            var request = new FakeRequest("GET", "/PrePost");
            var context = new NancyContext { Request = request };

            // When
            var result = specificResolver.Resolve(context, routeCache);

            // Then
            result.Item3.ShouldNotBeNull();
            result.Item4.ShouldNotBeNull();
        }
    }
}
