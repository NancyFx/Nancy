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

        [Fact]
        public void Should_locate_view_when_only_name_is_provided()
        {
            // Given
            var expectedView = new ViewLocationResult(string.Empty, "index", string.Empty, () => null);
            var cache = new FakeViewLocationCache(expectedView);

            var locator = CreateViewLocator(cache);

            // When
            var result = locator.LocateView("index");

            // Then
            result.ShouldBeSameAs(expectedView);
        }

        [Theory]
        [InlineData("INDEX")]
        [InlineData("InDEx")]
        public void Should_ignore_case_when_locating_view_based_on_name(string viewName)
        {
            // Given
            var expectedView = new ViewLocationResult(string.Empty, "index", string.Empty, () => null);
            var cache = new FakeViewLocationCache(expectedView);
            var locator = CreateViewLocator(cache);

            // When
            var result = locator.LocateView(viewName);

            // Then
            result.ShouldBeSameAs(expectedView);
        }

        [Fact]
        public void Should_throw_ambiguousviewsexception_when_locating_view_by_name_returns_multiple_results()
        {
            // Given
            var expectedView1 = new ViewLocationResult(string.Empty, "index", string.Empty, () => null);
            var expectedView2 = new ViewLocationResult(string.Empty, "index", string.Empty, () => null);
            var cache = new FakeViewLocationCache(expectedView1, expectedView2);
            var locator = CreateViewLocator(cache);

            // When
            var exception = Record.Exception(() => locator.LocateView("index"));

            // Then
            exception.ShouldBeOfType<AmbiguousViewsException>();
        }

        [Fact]
        public void Should_throw_ambiguousviewsexception_when_locating_view_by_name_and_multiple_views_share_the_same_name_and_location_but_different_extensions()
        {
            // Given
            var expectedView1 = new ViewLocationResult(string.Empty, "index", "spark", () => null);
            var expectedView2 = new ViewLocationResult(string.Empty, "index", "html", () => null);
            var cache = new FakeViewLocationCache(expectedView1, expectedView2);
            var locator = CreateViewLocator(cache);

            // When
            var exception = Record.Exception(() => locator.LocateView("index"));

            // Then
            exception.ShouldBeOfType<AmbiguousViewsException>();
        }

        [Fact]
        public void Should_set_message_on_ambiguousviewexception()
        {
            // Given
            var expectedView1 = new ViewLocationResult(string.Empty, "index", "spark", () => null);
            var expectedView2 = new ViewLocationResult(string.Empty, "index", "html", () => null);
            var cache = new FakeViewLocationCache(expectedView1, expectedView2);
            var locator = CreateViewLocator(cache);

            const string expectedMessage = "This exception was thrown because multiple views were found. 2 view(s):\r\n\t/index.spark\r\n\t/index.html";

            // When
            var exception = Record.Exception(() => locator.LocateView("index"));

            // Then
            exception.Message.ShouldEqual(expectedMessage);
        }

        [Fact]
        public void Should_return_null_when_view_cannot_be_located_using_name()
        {
            // Given
            var expectedView = new ViewLocationResult(string.Empty, "index", string.Empty, () => null);
            var cache = new FakeViewLocationCache(expectedView);

            var locator = CreateViewLocator(cache);

            // When
            var result = locator.LocateView("main");

            // Then
            result.ShouldBeNull();
        }

        [Fact]
        public void Should_locate_view_when_name_and_extension_are_provided()
        {
            // Given
            var expectedView = new ViewLocationResult(string.Empty, "index", "cshtml", () => null);
            var cache = new FakeViewLocationCache(expectedView);

            var locator = CreateViewLocator(cache);

            // When
            var result = locator.LocateView("index.cshtml");

            // Then
            result.ShouldBeSameAs(expectedView);
        }

        [Theory]
        [InlineData("INDEX.CSHTML")]
        [InlineData("InDEx.csHTml")]
        public void Should_ignore_case_when_locating_view_based_on_name_and_extension(string viewName)
        {
            // Given
            var expectedView = new ViewLocationResult(string.Empty, "index", "cshtml", () => null);
            var cache = new FakeViewLocationCache(expectedView);
            var locator = CreateViewLocator(cache);

            // When
            var result = locator.LocateView(viewName);

            // Then
            result.ShouldBeSameAs(expectedView);
        }

        [Fact]
        public void Should_return_null_when_view_cannot_be_located_using_name_and_extension()
        {
            // Given
            var expectedView = new ViewLocationResult(string.Empty, "index", "spark", () => null);
            var cache = new FakeViewLocationCache(expectedView);

            var locator = CreateViewLocator(cache);

            // When
            var result = locator.LocateView("index.cshtml");

            // Then
            result.ShouldBeNull();
        }

        [Fact]
        public void Should_locate_view_when_name_extension_and_location_are_provided()
        {
            // Given
            var expectedView = new ViewLocationResult("views/sub", "index", "cshtml", () => null);
            var cache = new FakeViewLocationCache(expectedView);

            var locator = CreateViewLocator(cache);

            // When
            var result = locator.LocateView("views/sub/index.cshtml");

            // Then
            result.ShouldBeSameAs(expectedView);
        }

        [Theory]
        [InlineData("VIEWS/SUB/INDEX.CSHTML")]
        [InlineData("viEWS/sUb/InDEx.csHTml")]
        public void Should_ignore_case_when_locating_view_based_on_name_extension_and_location(string viewName)
        {
            // Given
            var expectedView = new ViewLocationResult("views/sub", "index", "cshtml", () => null);
            var cache = new FakeViewLocationCache(expectedView);
            var locator = CreateViewLocator(cache);

            // When
            var result = locator.LocateView(viewName);

            // Then
            result.ShouldBeSameAs(expectedView);
        }

        [Fact]
        public void Should_return_null_when_view_cannot_be_located_using_name_extension_and_location()
        {
            // Given
            var expectedView = new ViewLocationResult("views/sub", "index", "spark", () => null);
            var cache = new FakeViewLocationCache(expectedView);

            var locator = CreateViewLocator(cache);

            // When
            var result = locator.LocateView("views/feature/index.cshtml");

            // Then
            result.ShouldBeNull();
        }

        [Fact]
        public void Should_be_able_to_locate_view_by_name_when_two_views_with_same_name_exists_at_different_locations()
        {
            // Given
            var expectedView = new ViewLocationResult("views/sub", "index", string.Empty, () => null);
            var additionalView = new ViewLocationResult("views", "index", string.Empty, () => null);
            var cache = new FakeViewLocationCache(expectedView, additionalView);
            var locator = CreateViewLocator(cache);

            // When
            var result = locator.LocateView("views/sub/index");

            // Then
            result.ShouldBeSameAs(expectedView);
        }

        [Fact]
        public void Should_be_able_to_locate_view_by_name_and_extension_when_two_view_with_same_name_but_different_extensions_exists_in_the_same_location()
        {
            // Given
            var expectedView = new ViewLocationResult("views", "index", "cshtml", () => null);
            var additionalView = new ViewLocationResult("views", "index", "spark", () => null);
            var cache = new FakeViewLocationCache(expectedView, additionalView);
            var locator = CreateViewLocator(cache);

            // When
            var result = locator.LocateView("views/index.cshtml");

            // Then
            result.ShouldBeSameAs(expectedView);
        }

        [Fact]
        public void Should_be_able_to_locate_view_by_name_when_two_views_with_same_name_and_extension_exists_at_different_locations()
        {
            // Given
            var expectedView = new ViewLocationResult("views/sub", "index", "cshtml", () => null);
            var additionalView = new ViewLocationResult("views", "index", "spark", () => null);
            var cache = new FakeViewLocationCache(expectedView, additionalView);
            var locator = CreateViewLocator(cache);

            // When
            var result = locator.LocateView("views/sub/index.cshtml");

            // Then
            result.ShouldBeSameAs(expectedView);
        }

        private static DefaultViewLocator CreateViewLocator(IViewLocationCache viewLocationCache)
        {
            return new DefaultViewLocator(viewLocationCache);
        }
    }
}