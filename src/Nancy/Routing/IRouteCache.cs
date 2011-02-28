namespace Nancy.Routing
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Contains a cache of all routes registered in the system
    /// </summary>
    public interface IRouteCache : IDictionary<string, List<Tuple<int, RouteDescription>>>
    {
        bool IsEmpty();
    }
}
