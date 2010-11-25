namespace Nancy.Tests.Unit
{
    using System;
    using System.Linq;
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
    }
}