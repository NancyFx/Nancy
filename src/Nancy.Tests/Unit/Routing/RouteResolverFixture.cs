namespace Nancy.Tests.Unit.Routing
{
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using Nancy;
    using Nancy.Extensions;
    using Nancy.Routing;
    using Nancy.Tests.Fakes;
    using Tests.Extensions;
    using Xunit;
    using Xunit.Extensions;

    public class RouteResolverFixture
    {
        private readonly IRouteResolver resolver;

        public RouteResolverFixture()
        {
            this.resolver = new RouteResolver();
        }

        [Fact]
        public void Should_return_no_matching_route_found_route_when_no_match_could_be_found()
        {
            // Given
            var request = new Request("GET", "/invalid", new Dictionary<string, IEnumerable<string>>(), new MemoryStream());

            var modules = new[] { new FakeNancyModuleWithBasePath() };
            var descriptions = modules.SelectMany(x => x.GetRouteDescription(request));

            // When
            var route = this.resolver.GetRoute(request, descriptions);

            // Then
            route.ShouldBeOfType<NoMatchingRouteFoundRoute>();
        }

        [Fact]
        public void Should_match_on_combination_of_module_base_path_and_action_path_when_module_defines_base_path()
        {
            // Given
            var request = new Request("GET", "/fake/route/with/some/parts", new Dictionary<string, IEnumerable<string>>(), new MemoryStream());

            var modules = new[] { new FakeNancyModuleWithBasePath() };
            var descriptions = modules.SelectMany(x => x.GetRouteDescription(request));

            // When
            var route = this.resolver.GetRoute(request, descriptions);

            // Then
            route.ShouldNotBeOfType<NoMatchingRouteFoundRoute>();
        }

        [Theory]
        [InlineData("/fake/Route/WITH/soMe/paRTs")]
        [InlineData("/FAKE/ROUTE/WITH/SOME/PARTS")]
        public void Should_be_case_insensitive_when_matching(string path)
        {
            // Given
            var request = new Request("GET", path, new Dictionary<string, IEnumerable<string>>(), new MemoryStream());

            var modules = new[] { new FakeNancyModuleWithBasePath() };
            var descriptions = modules.SelectMany(x => x.GetRouteDescription(request));

            // When
            var route = this.resolver.GetRoute(request, descriptions);

            // Then
            route.ShouldNotBeOfType<NoMatchingRouteFoundRoute>();
        }

        [Fact]
        public void Should_not_match_on_combination_of_module_base_path_and_action_path_when_module_defines_base_path()
        {
            // Given
            var request = new Request("GET", "/route/with/some/parts", new Dictionary<string, IEnumerable<string>>(), new MemoryStream());

            var modules = new[] { new FakeNancyModuleWithBasePath() };
            var descriptions = modules.SelectMany(x => x.GetRouteDescription(request));

            // When
            var route = this.resolver.GetRoute(request, descriptions);

            // Then
            route.ShouldBeOfType<NoMatchingRouteFoundRoute>();
        }

        [Fact]
        public void Should_set_combination_of_module_base_path_and_action_path_on_no_matching_route_found_route_when_no_match_could_be_found()
        {
            // Given
            var request = new Request("GET", "/fake/route/with/some/parts", new Dictionary<string, IEnumerable<string>>(), new MemoryStream());
            var modules = new[] { new FakeNancyModuleWithBasePath() };
            var descriptions = modules.SelectMany(x => x.GetRouteDescription(request));

            // When
            var route = this.resolver.GetRoute(request, descriptions);

            // Then
            route.Path.ShouldEqual(request.Uri);
        }

        [Fact]
        public void Should_set_action_on_route_when_match_was_found()
        {
            // Given
            var request = new Request("GET", "/fake/route/with/some/parts", new Dictionary<string, IEnumerable<string>>(), new MemoryStream());
            var modules = new[] { new FakeNancyModuleWithBasePath() };
            var descriptions = modules.SelectMany(x => x.GetRouteDescription(request));

            // When
            var route = this.resolver.GetRoute(request, descriptions);
            var response = route.Invoke();
            var output = response.GetStringContentsFromResponse();

            // Then
            output.ShouldEqual("FakeNancyModuleWithBasePath");
        }

        [Fact]
        public void Should_return_first_matched_route_when_conflicting_routs_are_available()
        {
            // Given
            var request = new Request("GET", "/fake/should/have/conflicting/route/defined", new Dictionary<string, IEnumerable<string>>(), new MemoryStream());
            var modules = new NancyModule[] { new FakeNancyModuleWithBasePath(), new FakeNancyModuleWithoutBasePath() };
            var descriptions = modules.SelectMany(x => x.GetRouteDescription(request));
            var route = this.resolver.GetRoute(request, descriptions);
            var response = route.Invoke();

            // When
            var output = response.GetStringContentsFromResponse();

            // Then
            output.ShouldEqual("FakeNancyModuleWithBasePath");
        }

        [Fact]
        public void Should_match_parameterized_action_path_with_request_path()
        {
            // Given
            var request = new Request("GET", "/fake/child/route", new Dictionary<string, IEnumerable<string>>(), new MemoryStream());
            var modules = new[] { new FakeNancyModuleWithBasePath() };
            var descriptions = modules.SelectMany(x => x.GetRouteDescription(request));

            // When
            var route = this.resolver.GetRoute(request, descriptions);

            // Then
            route.ShouldNotBeOfType<NoMatchingRouteFoundRoute>();
        }

        [Fact]
        public void Should_treat_action_route_parameters_as_greedy()
        {
            // Given
            var request = new Request("GET", "/fake/foo/some/stuff/not/in/route/bar/more/stuff/not/in/route", new Dictionary<string, IEnumerable<string>>(), new MemoryStream());
            var modules = new[] { new FakeNancyModuleWithBasePath() };
            var descriptions = modules.SelectMany(x => x.GetRouteDescription(request));

            // When
            var route = this.resolver.GetRoute(request, descriptions);

            // Then
            route.ShouldNotBeOfType<NoMatchingRouteFoundRoute>();
        }

        [Fact]
        public void Should_return_the_route_with_most_static_matches_when_multiple_matches_are_found()
        {
            // Given
            var request = new Request("GET", "/fake/child/route/foo", new Dictionary<string, IEnumerable<string>>(), new MemoryStream());
            var modules = new[] { new FakeNancyModuleWithBasePath() };
            var descriptions = modules.SelectMany(x => x.GetRouteDescription(request));
            var route = this.resolver.GetRoute(request, descriptions);
            var response = route.Invoke();

            // When
            var output = response.GetStringContentsFromResponse();

            // Then
            output.ShouldEqual("test");
        }

        [Fact]
        public void Should_set_parameters_on_route_when_match_was_made_for_parameterized_action_route()
        {
            // Given
            var request = new Request("GET", "/fake/foo/some/stuff/not/in/route/bar/more/stuff/not/in/route", new Dictionary<string, IEnumerable<string>>(), new MemoryStream());
            var modules = new[] { new FakeNancyModuleWithBasePath() };
            var descriptions = modules.SelectMany(x => x.GetRouteDescription(request));
            dynamic result;

            // When
            var route = this.resolver.GetRoute(request, descriptions);

            // Then
            Record.Exception(() => result = route.Parameters.value).ShouldBeNull();
            Record.Exception(() => result = route.Parameters.capture).ShouldBeNull();
        }

        protected static string GetStringContentsFromResponse(Response response)
        {
            var memory = new MemoryStream();
            response.Contents.Invoke(memory);
            memory.Position = 0;
            using (var reader = new StreamReader(memory))
            {
                return reader.ReadToEnd();
            }
        }
    }
}