namespace Nancy.Tests.Unit.Routing
{
    using System.Linq;
    using Nancy.Tests.Fakes;
    using Xunit;
    using FakeItEasy;
    using Nancy.Routing;

    public class RouteCacheFixture
    {
        private readonly INancyModuleCatalog fakeModuleCatalog;
        private readonly IRouteCache routeCache;
        private readonly IRouteSegmentExtractor routeSegmentExtractor;

        /// <summary>
        /// Initializes a new instance of the RouteCacheFixture class.
        /// </summary>
        public RouteCacheFixture()
        {
            this.routeSegmentExtractor = A.Fake<IRouteSegmentExtractor>();
            this.fakeModuleCatalog = new FakeModuleCatalog();

            this.routeCache = 
                new RouteCache(this.fakeModuleCatalog, new FakeModuleKeyGenerator(), A.Fake<INancyContextFactory>(), this.routeSegmentExtractor);
        }

        [Fact]
        public void Should_Contain_Entries_For_All_Modules()
        {
            // Given, When
            var routes = (from cacheEntry in this.routeCache.Keys
                         select cacheEntry).ToList();

            // Then
            routes.Contains("1").ShouldBeTrue();
            routes.Contains("2").ShouldBeTrue();
        }

        [Fact]
        public void Should_Contain_Entries_For_All_Routes()
        {
            // Given
            var total = this.fakeModuleCatalog.GetAllModules(new NancyContext()).Sum(nm => nm.Routes.Count());

            // When
            var cacheEntriesTotal = this.routeCache.Values.Sum(c => c.Count());

            // Then
            cacheEntriesTotal.ShouldEqual(total);
        }

        [Fact]
        public void Sets_Filter_If_Specified()
        {
            // Given
            var routes = from cacheEntry in this.routeCache.Values
                         from route in cacheEntry
                         where route.Item2.Path == "/filtered"
                         select route.Item2;

            // When
            var filteredRoute = routes.First();

            // Then
            filteredRoute.Condition.ShouldNotBeNull();
        }

        [Fact]
        public void Filter_Is_Null_If_Not_Specified()
        {
            // Given
            var routes = from cacheEntry in this.routeCache.Values
                         from route in cacheEntry
                         where route.Item2.Path == "/"
                         select route.Item2;

            // When
            var filteredRoute = routes.First();

            // Then
            filteredRoute.Condition.ShouldBeNull();
        }

        [Fact]
        public void Sets_Method()
        {
            // Given, When
            var methods = (from cacheEntry in routeCache.Values
                          from route in cacheEntry
                          select route.Item2.Method).Distinct();

            // Then
            methods.Count().ShouldEqual(4);
        }

        [Fact]
        public void Index_Set_Correctly_In_Cache()
        {
            // Given
            var routes = this.fakeModuleCatalog
                .GetModuleByKey("1", new NancyContext()).Routes.Select(r => r.Description)
                .ToList();

            // When
            var cachedRoutes = this.routeCache["1"];

            // Then
            foreach (var cachedRoute in cachedRoutes)
            {
                var index = cachedRoute.Item1;
                cachedRoute.Item2.ShouldBeSameAs(routes.ElementAt(index));
            }
        }
    }
}
