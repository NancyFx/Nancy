using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Nancy.Tests.Fakes;
using Xunit;

namespace Nancy.Tests.Unit.Routing
{
    public class RouteCacheFixture
    {
        private Nancy.INancyModuleCatalog _FakeModuleCatalog;
        private Nancy.Routing.IRouteCache _RouteCache;

        /// <summary>
        /// Initializes a new instance of the RouteCacheFixture class.
        /// </summary>
        public RouteCacheFixture()
        {
            _FakeModuleCatalog = new FakeModuleCatalog();

            _RouteCache = new Nancy.Routing.RouteCache(_FakeModuleCatalog, new FakeModuleKeyGenerator());
        }

        [Fact]
        public void Should_Contain_Entries_For_All_Modules()
        {
            var routes = from cacheEntry in _RouteCache.Keys
                         select cacheEntry;

            routes.Contains("1").ShouldBeTrue();
            routes.Contains("2").ShouldBeTrue();
        }

        [Fact]
        public void Should_Contain_Entries_For_All_Routes()
        {
            var total = _FakeModuleCatalog.GetAllModules().Sum(nm => nm.Routes.Count());

            var cacheEntriesTotal = _RouteCache.Values.Sum(c => c.Count());

            cacheEntriesTotal.ShouldEqual(total);
        }

        [Fact]
        public void Sets_Filter_If_Specified()
        {
            var routes = from cacheEntry in _RouteCache.Values
                         from route in cacheEntry
                         where route.Item2.Path == "/filtered"
                         select route.Item2;

            var filteredRoute = routes.First();

            filteredRoute.Condition.ShouldNotBeNull();
        }

        [Fact]
        public void Filter_Is_Null_If_Not_Specified()
        {
            var routes = from cacheEntry in _RouteCache.Values
                         from route in cacheEntry
                         where route.Item2.Path == "/"
                         select route.Item2;

            var filteredRoute = routes.First();

            filteredRoute.Condition.ShouldBeNull();
        }

        [Fact]
        public void Sets_Method()
        {
            var methods = (from cacheEntry in _RouteCache.Values
                          from route in cacheEntry
                          select route.Item2.Method).Distinct();

            methods.Count().ShouldEqual(4);
        }

        [Fact]
        public void Index_Set_Correctly_In_Cache()
        {
            var routes = _FakeModuleCatalog.GetModuleByKey("1").Routes.Select(r => r.Description);

            var cachedRoutes = _RouteCache["1"];

            foreach (var cachedRoute in cachedRoutes)
            {
                var index = cachedRoute.Item1;
                cachedRoute.Item2.ShouldBeSameAs(routes.ElementAt(index));
            }
        }
    }
}
