namespace Nancy.Tests.Fakes
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using Nancy.Routing;

    public class FakeRouteCache : Dictionary<string, List<Tuple<int, RouteDescription>>>, IRouteCache
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

            private void AddRoutesToCache(IEnumerable<RouteDescription> routes, string moduleKey)
            {
                if (!this.routeCache.ContainsKey(moduleKey))
                {
                    this.routeCache[moduleKey] = new List<Tuple<int, RouteDescription>>();
                }

                this.routeCache[moduleKey].AddRange(routes.Select((r, i) => new Tuple<int, RouteDescription>(i, r)));
            }

            public FakeRouteCacheConfigurator AddDeleteRoute(string path)
            {
                this.AddRoutesToCache(new[] { new RouteDescription("DELETE", path, null) }, String.Empty);
                return this;
            }

            public FakeRouteCacheConfigurator AddGetRoute(string path)
            {
                this.AddRoutesToCache(new[] { new RouteDescription("GET", path, null) }, String.Empty);
                
                return this;
            }

            public FakeRouteCacheConfigurator AddGetRoute(string path, string moduleKey)
            {
                this.AddRoutesToCache(new[] { new RouteDescription("GET", path, null) }, moduleKey);

                return this;
            }

            public FakeRouteCacheConfigurator AddGetRoute(string path, string moduleKey, Func<NancyContext, bool> condition)
            {
                this.AddRoutesToCache(new[] { new RouteDescription("GET", path, condition) }, moduleKey);

                return this;
            }

            public FakeRouteCacheConfigurator AddPostRoute(string path)
            {
                this.AddRoutesToCache(new[] { new RouteDescription("POST", path, null) }, String.Empty);

                return this;
            }

            public FakeRouteCacheConfigurator AddPutRoute(string path)
            {
                this.AddRoutesToCache(new[] { new RouteDescription("PUT", path, null) }, String.Empty);

                return this;
            }

            public FakeRouteCacheConfigurator AddOptionsRoute(string path)
            {
                this.AddRoutesToCache(new [] { new RouteDescription("OPTIONS", path, null)  }, String.Empty );

                return this;
            }
        }
    }
}