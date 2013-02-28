namespace Nancy.Routing
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Contains a cache of all routes registered in the system
    /// </summary>
    public interface IRouteCache : IDictionary<Type, List<Tuple<int, RouteDescription>>>
    {
        /// <summary>
        /// Gets a boolean value that indicates of the cache is empty or not.
        /// </summary>
        /// <returns><see langword="true"/> if the cache is empty, otherwise <see langword="false"/>.</returns>
        bool IsEmpty();
    }
}
