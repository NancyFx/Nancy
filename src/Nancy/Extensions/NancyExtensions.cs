namespace Nancy.Extensions
{
    using System.Collections.Generic;
    using System.Linq;
    using Nancy.Routing;

    public static class NancyExtensions
    {
        public static IEnumerable<RouteDescription> GetRouteDescription(this NancyModule source, IRequest request)
        {
            return source.GetRoutes(request.Method).Select(route => new RouteDescription { Action = route.Value, ModulePath = source.ModulePath, Path = route.Key });
        }
    }
}