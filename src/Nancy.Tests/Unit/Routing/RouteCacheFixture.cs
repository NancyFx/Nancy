namespace Nancy.Tests.Unit.Routing
{
    using System.Linq;
    using Nancy.Tests.Fakes;
    using Xunit;
    using FakeItEasy;
    using Nancy.Routing;
    using Nancy.Culture;

    public class RouteCacheFixture
    {
        private readonly INancyModuleCatalog fakeModuleCatalog;
        private readonly IRouteCache routeCache;
        private readonly IRouteSegmentExtractor routeSegmentExtractor;
        private readonly IRouteDescriptionProvider routeDescriptionProvider;

        /// <summary>
        /// Initializes a new instance of the RouteCacheFixture class.
        /// </summary>
        public RouteCacheFixture()
        {
            this.routeDescriptionProvider = A.Fake<IRouteDescriptionProvider>();
            this.routeSegmentExtractor = A.Fake<IRouteSegmentExtractor>();
            this.fakeModuleCatalog = new FakeModuleCatalog();

            this.routeCache =
                new RouteCache(this.fakeModuleCatalog, new FakeModuleKeyGenerator(), A.Fake<INancyContextFactory>(), this.routeSegmentExtractor, this.routeDescriptionProvider, A.Fake<ICultureService>());
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

        [Fact]
        public void Should_invoke_route_description_provider_with_module_that_route_is_defined_in()
        {
            // Given
            var module = new FakeNancyModule(with =>
            {
                with.AddGetRoute("/");
            });

            var catalog = A.Fake<INancyModuleCatalog>();
            A.CallTo(() => catalog.GetAllModules(A<NancyContext>._)).Returns(new[] { module });

            var descriptionProvider =
                A.Fake<IRouteDescriptionProvider>();

            // When
            var cache = new RouteCache(
                catalog,
                new FakeModuleKeyGenerator(),
                A.Fake<INancyContextFactory>(),
                this.routeSegmentExtractor,
                descriptionProvider,
                A.Fake<ICultureService>());

            // Then
            A.CallTo(() => descriptionProvider.GetDescription(module, A<string>._)).MustHaveHappened();
        }

        [Fact]
        public void Should_invoke_route_description_provider_with_path_of_route()
        {
            // Given
            const string expectedPath = "/some/path/{capture}";

            var module = new FakeNancyModule(with =>
            {
                with.AddGetRoute(expectedPath);
            });

            var catalog = A.Fake<INancyModuleCatalog>();
            A.CallTo(() => catalog.GetAllModules(A<NancyContext>._)).Returns(new[] { module });

            var descriptionProvider =
                A.Fake<IRouteDescriptionProvider>();

            // When
            var cache = new RouteCache(
                catalog,
                new FakeModuleKeyGenerator(),
                A.Fake<INancyContextFactory>(),
                this.routeSegmentExtractor,
                descriptionProvider,
                A.Fake<ICultureService>());

            // Then
            A.CallTo(() => descriptionProvider.GetDescription(A<NancyModule>._, expectedPath)).MustHaveHappened();
        }
    }
}
