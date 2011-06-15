namespace Nancy.Tests.Unit.ViewEngines
{
    using System.Collections.Generic;
    using System.Linq;
    using FakeItEasy;
    using Nancy.ViewEngines;
    using Xunit;

    public class DefaultViewLocationCacheFixture
    {
        [Fact]
        public void Should_return_views_that_matches_criterion()
        {
            // Given
            var engines = new[] { A.Fake<IViewEngine>(), A.Fake<IViewEngine>() };
            A.CallTo(() => engines[0].Extensions).Returns(new[] { "html" });
            A.CallTo(() => engines[1].Extensions).Returns(new[] { "spark" });

            var providers = new[] { A.Fake<IViewLocationProvider>(), A.Fake<IViewLocationProvider>() };
            A.CallTo(() => providers[0].GetLocatedViews(A<IEnumerable<string>>.Ignored))
                .Returns(new[] { new ViewLocationResult(string.Empty, string.Empty, string.Empty, null), new ViewLocationResult(string.Empty, string.Empty, string.Empty, null) });

            A.CallTo(() => providers[1].GetLocatedViews(A<IEnumerable<string>>.Ignored))
                .Returns(new[] { new ViewLocationResult(string.Empty, string.Empty, string.Empty, null) });

            var cache = CreateViewLocationCache(providers, engines);

            // When
            var result = cache.Where(x => true);

            // Then
            result.Count().ShouldEqual(3);
        }

        [Fact]
        public void Should_call_view_location_providers_with_available_extensions_when_created()
        {
            // Given
            var viewEngine1 = A.Fake<IViewEngine>();
            A.CallTo(() => viewEngine1.Extensions).Returns(new[] { "html" });

            var viewEngine2 = A.Fake<IViewEngine>();
            A.CallTo(() => viewEngine2.Extensions).Returns(new[] { "spark" });

            var viewLocationProvider = A.Fake<IViewLocationProvider>();
            var expectedViewEngineExtensions = new[] { "html", "spark" };

            // When
            CreateViewLocationCache(
                new[] { viewLocationProvider },
                new[] { viewEngine1, viewEngine2 });

            // Then
            A.CallTo(() => viewLocationProvider.GetLocatedViews(A<IEnumerable<string>>.That.Matches(
                    x => x.All(y => expectedViewEngineExtensions.Contains(y))))).MustHaveHappened();
        }

        [Fact]
        public void Should_call_view_location_providers_with_distinct_available_extensions_when_created()
        {
            // Given
            var viewEngine1 = A.Fake<IViewEngine>();
            A.CallTo(() => viewEngine1.Extensions).Returns(new[] { "html" });

            var viewEngine2 = A.Fake<IViewEngine>();
            A.CallTo(() => viewEngine2.Extensions).Returns(new[] { "spark", "html" });

            var viewLocationProvider = A.Fake<IViewLocationProvider>();
            var expectedViewEngineExtensions = new[] { "html", "spark" };

            // When
            CreateViewLocationCache(
                new[] { viewLocationProvider },
                new[] { viewEngine1, viewEngine2 });

            // Then
            A.CallTo(() => viewLocationProvider.GetLocatedViews(A<IEnumerable<string>>.That.Matches(
                x => expectedViewEngineExtensions.All(y => x.Where(z => z.Equals(y)).Count() == 1)))).MustHaveHappened();
        }

        [Fact]
        public void Should_call_all_view_location_providers_when_created()
        {
            // Given
            var viewEngine = A.Fake<IViewEngine>();
            A.CallTo(() => viewEngine.Extensions).Returns(new[] { "html" });

            var viewLocationProvider1 = A.Fake<IViewLocationProvider>();
            var viewLocationProvider2 = A.Fake<IViewLocationProvider>();

            // When
            CreateViewLocationCache(
                new[] { viewLocationProvider1, viewLocationProvider2 },
                new[] { viewEngine });

            // Then
            A.CallTo(() => viewLocationProvider1.GetLocatedViews(A<IEnumerable<string>>.Ignored)).MustHaveHappened();
            A.CallTo(() => viewLocationProvider2.GetLocatedViews(A<IEnumerable<string>>.Ignored)).MustHaveHappened();
        }

        private static DefaultViewLocationCache CreateViewLocationCache(IEnumerable<IViewLocationProvider> viewLocationProviders, IEnumerable<IViewEngine> viewEngines)
        {
            return new DefaultViewLocationCache(viewLocationProviders, viewEngines);
        }
    }
}