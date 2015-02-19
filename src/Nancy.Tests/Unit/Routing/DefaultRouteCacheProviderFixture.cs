namespace Nancy.Tests.Unit.Routing
{
    using FakeItEasy;

    using Nancy.Routing;

    using Xunit;

    public class DefaultRouteCacheProviderFixture
    {
        private IRouteCacheProvider _Provider;
        private IRouteCache _RouteCache;

        /// <summary>
        /// Initializes a new instance of the DefaultRouteCacheProviderFixture class.
        /// </summary>
        public DefaultRouteCacheProviderFixture()
        {
            _RouteCache = A.Fake<IRouteCache>();
            _Provider = new DefaultRouteCacheProvider(() => _RouteCache);
        }

        [Fact]
        public void Should_Return_Instance_Supplied_By_Func()
        {
            var result = _Provider.GetCache();

            result.ShouldBeSameAs(_RouteCache);
        }
    }
}
