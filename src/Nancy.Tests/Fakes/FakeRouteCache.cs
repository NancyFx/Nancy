namespace Nancy.Tests.Fakes
{
    using System;
    using System.Collections;
    using System.Collections.Generic;

    using Nancy.Routing;

    public class FakeRouteCache : IRouteCache
    {
        public static FakeRouteCache Empty = new FakeRouteCache();
        private readonly List<RouteDescription> cache = new List<RouteDescription>();

        public FakeRouteCache()
        {
        }

        public FakeRouteCache(Action<FakeRouteCacheConfigurator> closure)
        {
            var configurator =
                new FakeRouteCacheConfigurator(this);

            closure.Invoke(configurator);
        }

        public IEnumerator<RouteDescription> GetEnumerator()
        {
            return this.cache.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.cache.GetEnumerator();
        }

        public class FakeRouteCacheConfigurator
        {
            private readonly FakeRouteCache routeCache;

            public FakeRouteCacheConfigurator(FakeRouteCache routeCache)
            {
                this.routeCache = routeCache;
            }

            public FakeRouteCacheConfigurator AddDeleteRoute(string path)
            {
                this.routeCache.cache.Add(new RouteDescription(
                    string.Empty, "DELETE", path, x => true));

                return this;
            }

            public FakeRouteCacheConfigurator AddGetRoute(string path)
            {
                return this.AddGetRoute(path, string.Empty);
            }

            public FakeRouteCacheConfigurator AddGetRoute(string path, string moduleKey)
            {
                return this.AddGetRoute(path, moduleKey, x => true);
            }

            public FakeRouteCacheConfigurator AddGetRoute(string path, string moduleKey, Func<Request, bool> condition)
            {
                this.routeCache.cache.Add(new RouteDescription(
                    moduleKey, "GET", path, condition));

                return this;
            }

            public FakeRouteCacheConfigurator AddPostRoute(string path)
            {
                this.routeCache.cache.Add(new RouteDescription(
                    string.Empty, "POST", path, x => true));

                return this;
            }

            public FakeRouteCacheConfigurator AddPutRoute(string path)
            {
                this.routeCache.cache.Add(new RouteDescription(
                    string.Empty, "PUT", path, x => true));

                return this;
            }
        }
    }
}