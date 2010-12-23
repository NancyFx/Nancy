namespace Nancy
{
    using System;
    using System.Collections.Generic;
    using Routing;

    public class ModuleMeta
    {
        public Type Type { get; set; }
        public IEnumerable<RouteDescription> RouteDescriptions { get; set; }

        public ModuleMeta(Type type, IEnumerable<RouteDescription> routeDescriptions)
        {
            Type = type;
            RouteDescriptions = routeDescriptions;
        }
    }
}