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
            var routes = _RouteCache.Select(ce => ce.ModuleKey).Distinct();

            routes.Contains("1").ShouldBeTrue();
            routes.Contains("2").ShouldBeTrue();
        }

        [Fact]
        public void Should_Contain_Entries_For_All_Routes()
        {
            var total = _FakeModuleCatalog.GetAllModules().Select(nm => nm.Get.GetRouteDescriptions().Count()
                                                                + nm.Delete.GetRouteDescriptions().Count() 
                                                                + nm.Post.GetRouteDescriptions().Count() 
                                                                + nm.Put.GetRouteDescriptions().Count())
                                                                .Sum();

            var cacheEntriesTotal = _RouteCache.Count();

            cacheEntriesTotal.ShouldEqual(total);
        }

        [Fact]
        public void Sets_Filter_If_Specified()
        {
            var filteredRoute = _RouteCache.Where(rc => rc.Path == "/filtered").First();

            filteredRoute.Condition.ShouldNotBeNull();
        }

        [Fact]
        public void Filter_Is_Null_If_Not_Specified()
        {
            var filteredRoute = _RouteCache.Where(rc => rc.Path == "/").First();

            filteredRoute.Condition.ShouldBeNull();
        }

        [Fact]
        public void Sets_Method()
        {
            var methods = _RouteCache.Select(rc => rc.Method).Distinct();

            methods.Count().ShouldEqual(4);
        }

    }
}
