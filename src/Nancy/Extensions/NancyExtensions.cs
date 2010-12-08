namespace Nancy.Extensions
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Nancy.Routing;

    public static class NancyExtensions
    {
        public static IEnumerable<RouteDescription> GetRouteDescription(this NancyModule source, IRequest request)
        {
        	var method = request.Method;
			if (method.ToUpperInvariant() == "HEAD")
			{
				method = "GET";
			}
            return source.GetRoutes(method).Select(route => new RouteDescription { Action = route.Value, ModulePath = source.ModulePath, Path = route.Key });

        }
    }
}