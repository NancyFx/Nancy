namespace Nancy
{
    using System;
    using System.Collections.Generic;
    using Routing;

    public class ModuleMetaOLD
    {
        public ModuleMetaOLD(Type type, IEnumerable<RouteDescription> routeDescriptions)
        {
            Type = type;
            RouteDescriptions = routeDescriptions;
        }

        public IEnumerable<RouteDescription> RouteDescriptions { get; set; }

        public Type Type { get; set; }
    }
}