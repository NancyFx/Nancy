namespace Nancy.Tests.Unit.ViewEngines
{
    using Fakes;
    using Nancy.ViewEngines;
    using Xunit;
    using Xunit.Extensions;

    public class DefaultViewLocatorFixture
    {
        private readonly IViewLocationCache viewLocationCache;
        private readonly ViewLocationResult viewLocation;
        private readonly DefaultViewLocator viewLocator;

        public DefaultViewLocatorFixture()
        {
            this.viewLocation = new ViewLocationResult("location", "view", "html", null);
            this.viewLocationCache = new FakeViewLocationCache(this.viewLocation);
            this.viewLocator = CreateViewLocator(this.viewLocationCache);
        }

        [Theory]
        [InlineData("view")]
        [InlineData("ViEw")]
        public void Should_ignore_casing_of_view_name_when_locating_a_view(string viewNameToTest)
        {
            // Given
            var viewName = string.Concat(viewNameToTest);

            // When
            var result = viewLocator.LocateView(viewName);

            // Then
            result.ShouldBeSameAs(this.viewLocation);
        }

        [Theory]
        [InlineData("HtMl")]
        [InlineData("html")]
        public void Should_ignore_caseing_of_view_extension_when_locating_a_view(string viewExtensionToTest)
        {
            // Given
            var viewName = string.Concat("view.", viewExtensionToTest);

            // When
            var result = viewLocator.LocateView(viewName);

            // Then
            result.ShouldBeSameAs(viewLocation);
        }

        [Fact]
        public void Should_ignore_extension_when_resolving_view_and_view_name_does_not_contain_extension()
        {
            // Given, When
            var result = viewLocator.LocateView("view");

            // Then
            result.ShouldBeSameAs(viewLocation);
        }

        [Fact]
        public void Should_throw_ambiguousviewsexception_when_more_than_one_located_view_matches_view_name()
        {
            // Given
            var cache = new FakeViewLocationCache(
                new ViewLocationResult("location", "view", "html", null),
                new ViewLocationResult("location", "view", "html", null));

            var locator = CreateViewLocator(cache);

            // When
            var exception = Record.Exception(() => locator.LocateView("view.html"));

            // Then
            exception.ShouldBeOfType<AmbiguousViewsException>();
        }

        [Fact]
        public void Should_return_null_when_no_located_view_matches_view_name()
        {
            // Given, When
            var result = viewLocator.LocateView("view2.html");

            // Then
            result.ShouldBeNull();
        }

        [Fact]
        public void Should_return_view_when_located_view_matches_view_name()
        {
            // Given, When
            var result = viewLocator.LocateView("view.html");

            // Then
            result.ShouldBeSameAs(viewLocation);
        }

        [Fact]
        public void Should_return_null_if_locate_view_is_invoked_with_null_view_name()
        {
            // Given
            string viewName = null;

            // When
            var result = this.viewLocator.LocateView(viewName);

            // Then
            result.ShouldBeNull();
        }

        [Fact]
        public void Should_return_null_if_locate_view_is_invoked_with_empty_view_name()
        {
            // Given
            var viewName = string.Empty;

            // When
            var result = this.viewLocator.LocateView(viewName);

            // Then
            result.ShouldBeNull();
        }

        private static DefaultViewLocator CreateViewLocator(IViewLocationCache viewLocationCache)
        {
            return new DefaultViewLocator(viewLocationCache);
        }
    }
}