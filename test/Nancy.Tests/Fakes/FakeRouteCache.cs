namespace Nancy.Tests.Fakes
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Nancy.Routing;

    public class FakeRouteCache : Dictionary<Type, List<Tuple<int, RouteDescription>>>, IRouteCache
    {
        public static FakeRouteCache Empty = new FakeRouteCache();

        public FakeRouteCache()
        {
        }

        public FakeRouteCache(Action<FakeRouteCacheConfigurator> closure)
        {
            var configurator =
                new FakeRouteCacheConfigurator(this);

            closure.Invoke(configurator);
        }

        public bool IsEmpty()
        {
            return false;
        }

        public class FakeRouteCacheConfigurator
        {
            private readonly FakeRouteCache routeCache;

            public FakeRouteCacheConfigurator(FakeRouteCache routeCache)
            {
                this.routeCache = routeCache;
            }

            private void AddRoutesToCache(IEnumerable<RouteDescription> routes, Type moduleType)
            {
                if (!this.routeCache.ContainsKey(moduleType))
                {
                    this.routeCache[moduleType] = new List<Tuple<int, RouteDescription>>();
                }

                this.routeCache[moduleType].AddRange(routes.Select((r, i) => new Tuple<int, RouteDescription>(i, r)));
            }

            public FakeRouteCacheConfigurator AddGetRoute(string path, Type moduleType)
            {
                this.AddRoutesToCache(new[] { new RouteDescription(string.Empty, "GET", path, null) }, moduleType);

                return this;
            }

            public FakeRouteCacheConfigurator AddGetRoute(string path, Type moduleType, Func<NancyContext, bool> condition)
            {
                this.AddRoutesToCache(new[] { new RouteDescription(string.Empty, "GET", path, condition) }, moduleType);

                return this;
            }
        }
    }
}