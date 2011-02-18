namespace Nancy.Routing
{
    /// <summary>
    /// It's not safe for a module to take a dependency on the cache (cyclic dependency)
    /// 
    /// We provide an IRouteCacheProvider instead.
    /// 
    /// It is *not* safe to call GetCache() inside a NancyModule constructor, although that shouldn't be necessary anyway.
    /// </summary>
    public interface IRouteCacheProvider
    {
        IRouteCache GetCache();
    }
}
