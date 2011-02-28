using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;
using FakeItEasy;

namespace Nancy.Tests.Unit.Routing
{
    public class DefaultRouteCacheProviderFixture
    {
        private Nancy.Routing.IRouteCacheProvider _Provider;
        private Nancy.Routing.IRouteCache _RouteCache;

        /// <summary>
        /// Initializes a new instance of the DefaultRouteCacheProviderFixture class.
        /// </summary>
        public DefaultRouteCacheProviderFixture()
        {
            _RouteCache = A.Fake<Nancy.Routing.IRouteCache>();
            _Provider = new Nancy.Routing.DefaultRouteCacheProvider(() => _RouteCache);
        }

        [Fact]
        public void Should_Return_Instance_Supplied_By_Func()
        {
            var result = _Provider.GetCache();

            result.ShouldBeSameAs(_RouteCache);
        }
    }
}
