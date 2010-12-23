namespace Nancy.Extensions
{    
    using System.Collections.Generic;
    using System.Linq;    
    using Nancy.Routing;

    public static class NancyExtensions
    {
        public static IEnumerable<RouteDescription> GetRouteDescription(this NancyModule source, string method)
        {
            return source.GetRoutes(method).Select(route => new RouteDescription { ModulePath = source.ModulePath, Path = route.Key, Method = method });
        }
    }
}
