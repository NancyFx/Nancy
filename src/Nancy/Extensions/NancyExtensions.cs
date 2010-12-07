namespace Nancy.Extensions
{
    using System.Collections.Generic;
    using Nancy.Routing;

    public static class NancyExtensions
    {
        public static IEnumerable<RouteDescription> GetRouteDescription(this NancyModule source, IRequest request)
        {
            foreach (var route in source.GetRoutes(request.Verb))
            {
                yield return new RouteDescription { Action = route.Value, ModulePath = source.ModulePath, Path = route.Key };
            }
            
        }
    }
}
