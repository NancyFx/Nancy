namespace Nancy.Tests.Fakes
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Nancy.Routing;

    public class FakeRouteResolver : IRouteResolver
    {
        public IRoute GetRoute(IRequest request, IEnumerable<RouteDescription> descriptions)
        {
            var description = descriptions.First();

            this.Action = description.Action;
            this.ModulePath = description.ModulePath;
            this.Path = description.Path;

            return new FakeRoute();
        }

        public Func<object, Response> Action { get; private set; }

        public string Path { get; private set; }

        public string ModulePath { get; private set; }
    }
}