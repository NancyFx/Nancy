namespace Nancy.Extensions
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using Nancy.Routing;

    public static class NancyExtensions
    {
        public static IEnumerable<RouteDescription> GetRouteDescription(this NancyModule source, IRequest request)
        {
            const BindingFlags flags =
                BindingFlags.Instance | BindingFlags.Public | BindingFlags.IgnoreCase;

            var property =
                typeof(NancyModule).GetProperty(request.Verb, flags);

            if (property == null)
                return Enumerable.Empty<RouteDescription>();

            return
                from route in property.GetValue(source, null) as IDictionary<string, Func<object, Response>>
                select new RouteDescription
                {
                    Action = route.Value,
                    ModulePath = source.ModulePath,
                    Path = route.Key
                };
        }
    }
}