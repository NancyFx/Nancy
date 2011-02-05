namespace Nancy.Routing
{
    using System.Collections.Generic;

    /// <summary>
    /// Contains a cache of all routes registered in the system
    /// </summary>
    public interface IRouteCache : IEnumerable<RouteCacheEntry>
    {
    }
}
