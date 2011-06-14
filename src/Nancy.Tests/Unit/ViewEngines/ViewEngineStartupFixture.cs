namespace Nancy.Tests.Unit.ViewEngines
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using FakeItEasy;
    using Nancy.ViewEngines;
    using Xunit;

    public class ViewEngineStartupFixture
    {
        private readonly IViewCache viewCache;
        private readonly IViewLocationCache viewLocationCache;

        public ViewEngineStartupFixture()
        {
            this.viewCache = A.Fake<IViewCache>();
            this.viewLocationCache = A.Fake<IViewLocationCache>();
        }

        [Fact]
        public void Should_invoke_initialize_on_each_view_engine()
        {
            // Given
            var engines = new[] { A.Fake<IViewEngine>(), A.Fake<IViewEngine>() };
            var startup = new ViewEngineStartup(engines, this.viewLocationCache, this.viewCache);

            // When
            startup.Initialize();

            // Then
            A.CallTo(() => engines[0].Initialize(A<ViewEngineStartupContext>.Ignored)).MustHaveHappened();
            A.CallTo(() => engines[1].Initialize(A<ViewEngineStartupContext>.Ignored)).MustHaveHappened();
        }

        [Fact]
        public void Should_invoke_intialize_on_engine_with_view_cache_set_on_context()
        {
            // Given
            var engines = new[] { A.Fake<IViewEngine>() };
            var startup = new ViewEngineStartup(engines, this.viewLocationCache, this.viewCache);

            // When
            startup.Initialize();

            // Then
            A.CallTo(() => engines[0].Initialize(A<ViewEngineStartupContext>.That.Matches(x => x.ViewCache.Equals(this.viewCache)))).MustHaveHappened();
        }

        [Fact]
        public void Should_retrieve_view_templates_that_engine_can_render_from_viewtemplatelocationresult_cache()
        {
            // Given
            var engines = new[] { A.Fake<IViewEngine>() };
            A.CallTo(() => engines[0].Extensions).Returns(new[] { "html", "spark" });

            var fakeViewLocationCache = new FakeViewLocationResultCache(
                new ViewLocationResult("", "", "html", null),
                new ViewLocationResult("", "", "spark", null),
                new ViewLocationResult("", "", "razor", null)
                );

            var startup = new ViewEngineStartup(engines, fakeViewLocationCache, this.viewCache);

            // When
            startup.Initialize();

            // Then
            fakeViewLocationCache.MatchingViewLocationResults.Count().ShouldEqual(2);
        }

        [Fact]
        public void Should_invoke_initialize_on_engine_with_matching_view_locations_set_on_context()
        {
            // Given
            var engines = new[] { A.Fake<IViewEngine>() };
            A.CallTo(() => engines[0].Extensions).Returns(new[] { "html", "spark" });

            var fakeViewLocationCache = new FakeViewLocationResultCache(
                new ViewLocationResult("", "", "html", null),
                new ViewLocationResult("", "", "spark", null)
                );

            var startup = new ViewEngineStartup(engines, fakeViewLocationCache, this.viewCache);

            // When
            startup.Initialize();

            // Then
            A.CallTo(() => engines[0].Initialize(A<ViewEngineStartupContext>.That.Matches(x => x.ViewLocationResults.Count().Equals(2)))).MustHaveHappened();
        }
    }

    class FakeViewLocationResultCache : IViewLocationCache
    {
        private readonly ViewLocationResult[] viewLocationResults;

        public FakeViewLocationResultCache(params ViewLocationResult[] viewLocationResults)
        {
            this.viewLocationResults = viewLocationResults;
            this.MatchingViewLocationResults = new List<ViewLocationResult>();
        }

        public List<ViewLocationResult> MatchingViewLocationResults { get; private set; }

        public IEnumerable<ViewLocationResult> GetMatchingViews(Func<ViewLocationResult, bool> criterion)
        {
            var matchedForCriterion = this.viewLocationResults.Where(criterion);

            this.MatchingViewLocationResults.AddRange(matchedForCriterion);

            return matchedForCriterion;
        }
    }
}