using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Nancy.BootStrapper;
using Nancy.Extensions;

namespace Nancy.Routing
{
    /// <summary>
    /// Contains a cache of all routes registered in the system
    /// </summary>
    public interface IRouteCache : IEnumerable<RouteCacheEntry>
    {
    }
}
