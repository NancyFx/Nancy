namespace Nancy.Routing
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Nancy.Diagnostics;

    /// <summary>
    /// It's not safe for a module to take a dependency on the cache (cyclic dependency)
    ///
    /// We provide an IRouteCacheProvider instead - the default implementation uses
    /// TinyIoC'd Func based lazy factory.
    /// </summary>
    public class DefaultRouteCacheProvider : IRouteCacheProvider, IDiagnosticsProvider
    {
        protected readonly Func<IRouteCache> RouteCacheFactory;

        /// <summary>
        /// Gets the name of the provider.
        /// </summary>
        /// <value>A <see cref="string"/> containing the name of the provider.</value>
        public string Name
        {
            get { return "Route Cache"; }
        }

        /// <summary>
        /// Gets the description of the provider.
        /// </summary>
        /// <value>A <see cref="string"/> containing the description of the provider.</value>
        public string Description
        {
            get { return "Provides methods for viewing and querying the route cache."; }
        }

        /// <summary>
        /// Gets the object that contains the interactive diagnostics methods.
        /// </summary>
        /// <value>An instance of the interactive diagnostics object.</value>
        public object DiagnosticObject { get; private set; }

        /// <summary>
        /// Initializes a new instance of the DefaultRouteCacheProvider class.
        /// </summary>
        /// <param name="routeCacheFactory"></param>
        public DefaultRouteCacheProvider(Func<IRouteCache> routeCacheFactory)
        {
            this.RouteCacheFactory = routeCacheFactory;

            this.DiagnosticObject = new RouteCacheDiagnostics(this);
        }

        /// <summary>
        /// Gets an instance of the route cache.
        /// </summary>
        /// <returns>An <see cref="IRouteCache"/> instance.</returns>
        public IRouteCache GetCache()
        {
            return this.RouteCacheFactory();
        }

        private class RouteCacheDiagnostics
        {
            private readonly DefaultRouteCacheProvider cacheProvider;

            public RouteCacheDiagnostics(DefaultRouteCacheProvider cacheProvider)
            {
                this.cacheProvider = cacheProvider;
            }

            // ReSharper disable once UnusedMember.Local
            public IDictionary<string, IList<object>> GetAllRoutes()
            {
                var result = new Dictionary<string, IList<object>>();

                foreach (var entry in this.cacheProvider.GetCache().Values.SelectMany(t => t.Select(t1 => t1.Item2)))
                {
                    if (!result.ContainsKey(entry.Method))
                    {
                        result[entry.Method] = new List<object>();
                    }

                    result[entry.Method].Add(new { Name = entry.Name, Path = entry.Path });
                }

                return result;
            }
        }
    }
}