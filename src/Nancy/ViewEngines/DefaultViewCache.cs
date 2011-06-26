namespace Nancy.ViewEngines
{
    using System;
    using System.Collections.Concurrent;

    /// <summary>
    /// Default view cache implementation.
    /// </summary>
    public class DefaultViewCache : IViewCache
    {
        private readonly ConcurrentDictionary<ViewLocationResult, object> cache;

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultViewCache"/> class.
        /// </summary>
        public DefaultViewCache()
        {
            this.cache = new ConcurrentDictionary<ViewLocationResult, object>();
        }

        /// <summary>
        /// Gets or adds a view from the cache.
        /// </summary>
        /// <typeparam name="TCompiledView">The type of the cached view instance.</typeparam>
        /// <param name="viewLocationResult">A <see cref="ViewLocationResult"/> instance that describes the view that is being added or retrieved from the cache.</param>
        /// <param name="valueFactory">A function that produces the value that should be added to the cache in case it does not already exist.</param>
        /// <returns>An instance of the type specified by the <typeparamref name="TCompiledView"/> type.</returns>
        public TCompiledView GetOrAdd<TCompiledView>(ViewLocationResult viewLocationResult, Func<ViewLocationResult, TCompiledView> valueFactory)
        {
            if (StaticConfiguration.DisableCaches)
            {
                return valueFactory(viewLocationResult);
            }

            return (TCompiledView)this.cache.GetOrAdd(viewLocationResult, (x) => valueFactory(x));
        }
    }
}