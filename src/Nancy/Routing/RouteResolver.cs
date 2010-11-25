namespace Nancy.Routing
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public interface IRouteResolver
    {
        IRoute GetRoute(IRequest request, IEnumerable<RouteDescription> descriptions);
    }

    public class RouteResolver : IRouteResolver
    {
        public IRoute GetRoute(IRequest request, IEnumerable<RouteDescription> descriptions)
        {
            var matches =
                from description in descriptions
                where description.Path.Equals(request.Route)
                select description;

            var match = matches.First();

            return new Route(string.Empty, null);
        }
    }
}