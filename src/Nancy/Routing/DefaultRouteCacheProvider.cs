using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nancy.Routing
{
    /// <summary>
    /// It's not safe for a module to take a dependency on the cache (cyclic dependency)
    /// 
    /// We provide an IRouteCacheProvider instead - the default implementation uses
    /// TinyIoC'd Func based lazy factory.
    /// </summary>
    public class DefaultRouteCacheProvider : IRouteCacheProvider
    {
        protected readonly Func<IRouteCache> _RouteCacheFactory;

        /// <summary>
        /// Initializes a new instance of the DefaultRouteCacheProvider class.
        /// </summary>
        /// <param name="routeCacheFactory"></param>
        public DefaultRouteCacheProvider(Func<IRouteCache> routeCacheFactory)
        {
            _RouteCacheFactory = routeCacheFactory;
        }

        public IRouteCache GetCache()
        {
            return _RouteCacheFactory();
        }
    }
}
