namespace Nancy.ViewEngines.Spark.Tests
{
    using System.Collections.Generic;
    using System.IO;
    using Configuration;
    using FakeItEasy;
    using Nancy.Tests;
    using Xunit;

    public class NancyViewFolderFixture
    {
        private readonly IViewCache cache;
        private readonly INancyEnvironment environment;

        public NancyViewFolderFixture()
        {
            this.cache = A.Fake<IViewCache>();
            this.environment = new DefaultNancyEnvironment();
            this.environment.AddValue(ViewConfiguration.Default);
        }

        [Fact]
        public void Should_throw_filenotfoundexception_when_view_source_cannot_be_returned_for_view()
        {
            // Given
            var viewFolder = CreateViewFolder(new ViewLocationResult(
                string.Empty,
                "view",
                "spark",
                () => new StreamReader(new MemoryStream())));

            // When
            var exception = Record.Exception(() => viewFolder.GetViewSource("notfound.spark"));

            // Then
            exception.ShouldBeOfType<FileNotFoundException>();
        }

        [Fact]
        public void Should_get_view_source_for_view_without_location()
        {
            // Given
            var viewFolder = CreateViewFolder(new ViewLocationResult(
                string.Empty,
                "view",
                "spark",
                () => new StreamReader(new MemoryStream())));

            // When
            var result = viewFolder.GetViewSource("view.spark");

            // Then
            result.ShouldNotBeNull();
        }

        [Fact]
        public void Should_get_view_source_for_view_with_location()
        {
            // Given
            var viewFolder = CreateViewFolder(new ViewLocationResult(
                "location",
                "view",
                "spark",
                () => new StreamReader(new MemoryStream())));

            // When
            var result = viewFolder.GetViewSource("location/view.spark");

            // Then
            result.ShouldNotBeNull();
        }

        [Fact]
        public void Should_return_true_for_hashview_when_matching_views_without_location()
        {
            // Given
            var viewFolder = CreateViewFolder(new ViewLocationResult(
                string.Empty,
                "view",
                "spark",
                () => new StreamReader(new MemoryStream())));

            // When
            var result = viewFolder.HasView("view.spark");

            // Then
            result.ShouldBeTrue();
        }

        [Fact]
        public void Should_return_true_for_hashview_when_matching_views_with_location()
        {
            // Given
            var viewFolder = CreateViewFolder(new ViewLocationResult(
                "location",
                "view",
                "spark",
                () => new StreamReader(new MemoryStream())));

            // When
            var result = viewFolder.HasView("location/view.spark");

            // Then
            result.ShouldBeTrue();
        }

        private NancyViewFolder CreateViewFolder(params ViewLocationResult[] results)
        {
            var context =
                this.CreateContext(results);

            return new NancyViewFolder(context, this.environment);
        }

        private ViewEngineStartupContext CreateContext(params ViewLocationResult[] results)
        {
            var viewLocationProvider = A.Fake<IViewLocationProvider>();
            A.CallTo(() => viewLocationProvider.GetLocatedViews(A<IEnumerable<string>>._))
                                               .Returns(results);

            var viewEngine = A.Fake<IViewEngine>();
            A.CallTo(() => viewEngine.Extensions).Returns(new[] { "liquid" });

            var viewLocator = new DefaultViewLocator(viewLocationProvider, new[] { viewEngine }, this.environment);

            var startupContext = new ViewEngineStartupContext(
                this.cache,
                viewLocator);

            return startupContext;
        }
    }
}
