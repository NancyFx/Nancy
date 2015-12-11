namespace Nancy.Tests.Unit.ViewEngines
{
    using System.Collections.Generic;

    using FakeItEasy;

    using Nancy.ViewEngines;

    using Xunit;

    public class ViewEngineStartupFixture
    {
        private IList<ViewLocationResult> views;
        private readonly IViewCache viewCache;

        private readonly IViewLocator viewLocator;

        public ViewEngineStartupFixture()
        {
            this.views = new List<ViewLocationResult>
            {
                new ViewLocationResult("", "", "html", null),
                new ViewLocationResult("", "", "spark", null),
            };

            var viewLocationProvider = A.Fake<IViewLocationProvider>();
            A.CallTo(() => viewLocationProvider.GetLocatedViews(A<IEnumerable<string>>._))
                                               .Returns(views);

            var viewEngine = A.Fake<IViewEngine>();
            A.CallTo(() => viewEngine.Extensions).Returns(new[] { "liquid" });

            this.viewLocator = new DefaultViewLocator(viewLocationProvider, new[] { viewEngine });

            this.viewCache = A.Fake<IViewCache>();
        }

        [Fact]
        public void Should_invoke_initialize_on_each_view_engine()
        {
            // Given
            var engines = new[] { A.Fake<IViewEngine>(), A.Fake<IViewEngine>() };
            var startup = new ViewEngineApplicationStartup(engines, this.viewCache, this.viewLocator);

            // When
            startup.Initialize(null);

            // Then
            A.CallTo(() => engines[0].Initialize(A<ViewEngineStartupContext>.Ignored)).MustHaveHappened();
            A.CallTo(() => engines[1].Initialize(A<ViewEngineStartupContext>.Ignored)).MustHaveHappened();
        }

        [Fact]
        public void Should_invoke_intialize_on_engine_with_view_cache_set_on_context()
        {
            // Given
            var engines = new[] { A.Fake<IViewEngine>() };
            var startup = new ViewEngineApplicationStartup(engines, this.viewCache, this.viewLocator);

            // When
            startup.Initialize(null);

            // Then
            A.CallTo(() => engines[0].Initialize(A<ViewEngineStartupContext>.That.Matches(x => x.ViewCache.Equals(this.viewCache)))).MustHaveHappened();
        }
    }
}