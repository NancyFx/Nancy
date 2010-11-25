namespace Nancy.Tests.Unit
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Extensions;
    using FakeItEasy;
    using Nancy;
    using Nancy.Routing;
    using Nancy.Tests.Fakes;
    using Xunit;

    public class RouteResolverFixture
    {
        private readonly IRouteResolver resolver;

        public RouteResolverFixture()
        {
            this.resolver = new RouteResolver();
        }

        [Fact]
        public void Should_match_basic_request_path_to_route()
        {
            //Given
            var request = new Request("GET", "/route/with/some/parts");

            var modules = new[] { new FakeNancyModule() };
            var descriptions = modules.SelectMany(x => x.GetRouteDescription(request));

            // When
            var route = this.resolver.GetRoute(request, descriptions);

            // Then
            route.ShouldNotBeNull();
        }

        /*
         * No paramters
         * /foo/bar/baz
         * 
         * Parameters
         * /foo/{parameter}
         * 
         * Empty route should turn into
         * /
         * 
         * Should select route with most static parts when overlapping routes are found
         * /foo/{value}
         * /foo/bar/{value}
         * 
         * Should be case insensitive
         * /Foo/bar
         * /fOo/bAr
         * 
         * Should use basepath if its available
         * 
         * Should set path on route
         * Should set action on route
         * Should set parameters on route
         */
    }
}