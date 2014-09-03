namespace Nancy.Routing
{
    /// <summary>
    /// It's not safe for a module to take a dependency on the cache (cyclic dependency)
    ///
    /// We provide an <see cref="IRouteCacheProvider"/> instead.
    ///
    /// It is *not* safe to call GetCache() inside a NancyModule constructor, although that shouldn't be necessary anyway.
    /// </summary>
    public interface IRouteCacheProvider
    {
        /// <summary>
        /// Gets an instance of the route cache.
        /// </summary>
        /// <returns>An <see cref="IRouteCache"/> instance.</returns>
        IRouteCache GetCache();
    }
}
