namespace Nancy.Tests.Unit.ViewEngines
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using FakeItEasy;
    using Nancy.ViewEngines;
    using Xunit;
    using Xunit.Extensions;

    public class DefaultViewLocatorFixture
    {
        [Theory]
        [InlineData("view")]
        [InlineData("ViEw")]
        public void Should_ignore_caseing_of_view_name_when_locating_a_view(string viewNameToTest)
        {
            // Given
            var viewLocation = new ViewLocationResult("location", "view", "html", GetEmptyContentReader());

            var viewEngine = A.Fake<IViewEngine>();
            A.CallTo(() => viewEngine.Extensions).Returns(new[] { "html" });

            var viewLocationProvider = A.Fake<IViewLocationProvider>();
            A.CallTo(() => viewLocationProvider.GetLocatedViews(A<IEnumerable<string>>.Ignored)).Returns(new[] { viewLocation });

            // When
            var viewLocator = CreateViewLocator(
                new[] { viewLocationProvider },
                new[] { viewEngine });

            var viewName = string.Concat(viewNameToTest, ".html");

            // When
            var result = viewLocator.LocateView(viewName);

            // Then
            result.ShouldBeSameAs(viewLocation);
        }

        [Theory]
        [InlineData("HtMl")]
        [InlineData("html")]
        public void Should_ignore_caseing_of_view_extension_when_locating_a_view(string viewExtensionToTest)
        {
            // Given
            var viewLocation = new ViewLocationResult("location", "view", "html", GetEmptyContentReader());

            var viewEngine = A.Fake<IViewEngine>();
            A.CallTo(() => viewEngine.Extensions).Returns(new[] { "html" });

            var viewLocationProvider = A.Fake<IViewLocationProvider>();
            A.CallTo(() => viewLocationProvider.GetLocatedViews(A<IEnumerable<string>>.Ignored)).Returns(new[] { viewLocation });

            // When
            var viewLocator = CreateViewLocator(
                new[] { viewLocationProvider },
                new[] { viewEngine });

            var viewName = string.Concat("view.", viewExtensionToTest);

            // When
            var result = viewLocator.LocateView(viewName);

            // Then
            result.ShouldBeSameAs(viewLocation);
        }

        [Fact]
        public void Should_ignore_extension_when_resolving_view_and_view_name_does_not_contain_extension()
        {
            // Given
            var viewLocation = new ViewLocationResult("location", "view", "html", GetEmptyContentReader());

            var viewEngine = A.Fake<IViewEngine>();
            A.CallTo(() => viewEngine.Extensions).Returns(new[] { "html" });

            var viewLocationProvider = A.Fake<IViewLocationProvider>();
            A.CallTo(() => viewLocationProvider.GetLocatedViews(A<IEnumerable<string>>.Ignored)).Returns(new[] { viewLocation });

            // When
            var viewLocator = CreateViewLocator(
                new[] { viewLocationProvider },
                new[] { viewEngine });

            // When
            var result = viewLocator.LocateView("view");

            // Then
            result.ShouldBeSameAs(viewLocation);
        }

        [Fact]
        public void Should_throw_ambiguousviewsexception_when_more_than_one_located_view_matches_view_name()
        {
            // Given
            var viewLocation1 = new ViewLocationResult("location", "view", "html", GetEmptyContentReader());
            var viewLocation2 = new ViewLocationResult("location", "view", "html", GetEmptyContentReader());

            var viewEngine = A.Fake<IViewEngine>();
            A.CallTo(() => viewEngine.Extensions).Returns(new[] { "html" });

            var viewLocationProvider = A.Fake<IViewLocationProvider>();
            A.CallTo(() => viewLocationProvider.GetLocatedViews(A<IEnumerable<string>>.Ignored)).Returns(new[] { viewLocation1, viewLocation2 });

            var viewLocator = CreateViewLocator(
                new[] { viewLocationProvider },
                new[] { viewEngine });

            // When
            var exception = Record.Exception(() => viewLocator.LocateView("view.html"));

            // Then
            exception.ShouldBeOfType<AmbiguousViewsException>();
        }

        [Fact]
        public void Should_return_null_when_no_located_view_matches_view_name()
        {
            // Given
            var viewLocation = new ViewLocationResult("location", "view", "html", GetEmptyContentReader());

            var viewEngine = A.Fake<IViewEngine>();
            A.CallTo(() => viewEngine.Extensions).Returns(new[] { "html" });

            var viewLocationProvider = A.Fake<IViewLocationProvider>();
            A.CallTo(() => viewLocationProvider.GetLocatedViews(A<IEnumerable<string>>.Ignored)).Returns(new[] { viewLocation });

            var viewLocator = CreateViewLocator(
                new[] { viewLocationProvider },
                new[] { viewEngine });

            // When
            var result = viewLocator.LocateView("view2.html");

            // Then
            result.ShouldBeNull();
        }

        [Fact]
        public void Should_return_view_when_located_view_matches_view_name()
        {
            // Given
            var viewLocation = new ViewLocationResult("location", "view", "html", GetEmptyContentReader());

            var viewEngine = A.Fake<IViewEngine>();
            A.CallTo(() => viewEngine.Extensions).Returns(new[] { "html" });

            var viewLocationProvider = A.Fake<IViewLocationProvider>();
            A.CallTo(() => viewLocationProvider.GetLocatedViews(A<IEnumerable<string>>.Ignored)).Returns(new[] { viewLocation });

            // When
            var viewLocator = CreateViewLocator(
                new[] { viewLocationProvider },
                new[] { viewEngine });

            // When
            var result = viewLocator.LocateView("view.html");

            // Then
            result.ShouldBeSameAs(viewLocation);
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
            CreateViewLocator(
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
            CreateViewLocator(
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
            CreateViewLocator(
                new[] { viewLocationProvider1, viewLocationProvider2 },
                new[] { viewEngine });

            // Then
            A.CallTo(() => viewLocationProvider1.GetLocatedViews(A<IEnumerable<string>>.Ignored)).MustHaveHappened();
            A.CallTo(() => viewLocationProvider2.GetLocatedViews(A<IEnumerable<string>>.Ignored)).MustHaveHappened();
        }

        [Fact]
        public void Should_return_null_if_locate_view_is_invoked_with_null_view_name()
        {
            // Given
            string viewName = null;

            var viewLocator = CreateViewLocator(
                new[] { A.Fake<IViewLocationProvider>() },
                new[] { A.Fake<IViewEngine>() });

            // When
            var result = viewLocator.LocateView(viewName);

            // Then
            result.ShouldBeNull();
        }
     
        [Fact]
        public void Should_return_null_if_locate_view_is_invoked_with_empty_view_name()
        {
            // Given
            var viewName = string.Empty;

            var viewLocator = CreateViewLocator(
                new[] { A.Fake<IViewLocationProvider>() },
                new[] { A.Fake<IViewEngine>() });

            // When
            var result = viewLocator.LocateView(viewName);

            // Then
            result.ShouldBeNull();
        }

        private static DefaultViewLocator CreateViewLocator(IEnumerable<IViewLocationProvider> viewLocationProviders, IEnumerable<IViewEngine> viewEngines)
        {
            return new DefaultViewLocator(viewLocationProviders, viewEngines);
        }

        private static Func<TextReader> GetEmptyContentReader()
        {
            return () => new StreamReader(new MemoryStream());
        }
    }
}