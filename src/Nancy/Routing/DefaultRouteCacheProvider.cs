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

        private object diagnosticObject;

        public string Name
        {
            get
            {
                return "Route Cache";
            }
        }

        public object DiagnosticObject
        {
            get
            {
                return this.diagnosticObject;
            }
        }

        /// <summary>
        /// Initializes a new instance of the DefaultRouteCacheProvider class.
        /// </summary>
        /// <param name="routeCacheFactory"></param>
        public DefaultRouteCacheProvider(Func<IRouteCache> routeCacheFactory)
        {
            this.RouteCacheFactory = routeCacheFactory;

            this.diagnosticObject = new RouteCacheDiagnostics(this);
        }

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

            public IDictionary<string, IList<string>> GetAllRoutes()
            {
                var result = new Dictionary<string, IList<string>>();

                foreach (var entry in this.cacheProvider.GetCache().Values.SelectMany(t => t.Select(t1 => t1.Item2)))
                {
                    if (!result.ContainsKey(entry.Method))
                    {
                        result[entry.Method] = new List<string>();
                    }

                    result[entry.Method].Add(entry.Path);
                }

                return result;
            }
        }
    }
}