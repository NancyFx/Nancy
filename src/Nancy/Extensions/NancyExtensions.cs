namespace Nancy.Extensions
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Nancy.Routing;

    public static class NancyExtensions
    {
        public static IEnumerable<RouteDescription> GetRouteDescription(this NancyModule source, string method)
        {
            return source.GetRoutes(method).GetRouteDescriptions();
        }
    }
}
