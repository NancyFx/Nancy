namespace Nancy.Tests.Unit.ViewEngines
{
    using System.Collections.Generic;
    using System.Linq;

    using FakeItEasy;

    using Nancy.ViewEngines;

    using Xunit;
    using Xunit.Extensions;

    public class DefaultViewLocatorFixture
    {
        private readonly ViewLocationResult viewLocation;
        private readonly DefaultViewLocator viewLocator;

        public DefaultViewLocatorFixture()
        {
            this.viewLocation = new ViewLocationResult("location", "view", "html", null);
            this.viewLocator = CreateViewLocator();
        }

        [Fact]
        public void Should_return_null_if_locate_view_is_invoked_with_null_view_name()
        {
            // Given
            string viewName = null;

            // When
            var result = this.viewLocator.LocateView(viewName, null);

            // Then
            result.ShouldBeNull();
        }

        [Fact]
        public void Should_return_null_if_locate_view_is_invoked_with_empty_view_name()
        {
            // Given
            var viewName = string.Empty;

            // When
            var result = this.viewLocator.LocateView(viewName, null);

            // Then
            result.ShouldBeNull();
        }

        [Fact]
        public void Should_locate_view_when_only_name_is_provided()
        {
            // Given
            var expectedView = new ViewLocationResult(string.Empty, "index", string.Empty, () => null);

            var locator = CreateViewLocator(expectedView);

            // When
            var result = locator.LocateView("index", null);

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
            var locator = CreateViewLocator(expectedView);

            // When
            var result = locator.LocateView(viewName, null);

            // Then
            result.ShouldBeSameAs(expectedView);
        }

        [Fact]
        public void Should_throw_ambiguousviewsexception_when_locating_view_by_name_returns_multiple_results()
        {
            // Given
            var expectedView1 = new ViewLocationResult(string.Empty, "index", string.Empty, () => null);
            var expectedView2 = new ViewLocationResult(string.Empty, "index", string.Empty, () => null);
            var locator = CreateViewLocator(expectedView1, expectedView2);

            // When
            var exception = Record.Exception(() => locator.LocateView("index", null));

            // Then
            exception.ShouldBeOfType<AmbiguousViewsException>();
        }

        [Fact]
        public void Should_throw_ambiguousviewsexception_when_locating_view_by_name_and_multiple_views_share_the_same_name_and_location_but_different_extensions()
        {
            // Given
            var expectedView1 = new ViewLocationResult(string.Empty, "index", "spark", () => null);
            var expectedView2 = new ViewLocationResult(string.Empty, "index", "html", () => null);
            var locator = CreateViewLocator(expectedView1, expectedView2);

            // When
            var exception = Record.Exception(() => locator.LocateView("index", null));

            // Then
            exception.ShouldBeOfType<AmbiguousViewsException>();
        }

        [Fact]
        public void Should_set_message_on_ambiguousviewexception()
        {
            // Given
            var expectedView1 = new ViewLocationResult(string.Empty, "index", "spark", () => null);
            var expectedView2 = new ViewLocationResult(string.Empty, "index", "html", () => null);
            var locator = CreateViewLocator(expectedView1, expectedView2);

            const string expectedMessage = "This exception was thrown because multiple views were found. 2 view(s):\r\n\t/index.spark\r\n\t/index.html";

            // When
            var exception = Record.Exception(() => locator.LocateView("index", null));

            // Then
            exception.Message.ShouldEqual(expectedMessage);
        }

        [Fact]
        public void Should_return_null_when_view_cannot_be_located_using_name()
        {
            // Given
            var expectedView = new ViewLocationResult(string.Empty, "index", string.Empty, () => null);

            var locator = CreateViewLocator(expectedView);

            // When
            var result = locator.LocateView("main", null);

            // Then
            result.ShouldBeNull();
        }

        [Fact]
        public void Should_locate_view_when_name_and_extension_are_provided()
        {
            // Given
            var expectedView = new ViewLocationResult(string.Empty, "index", "cshtml", () => null);

            var locator = CreateViewLocator(expectedView);

            // When
            var result = locator.LocateView("index.cshtml", null);

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
            var locator = CreateViewLocator(expectedView);

            // When
            var result = locator.LocateView(viewName, null);

            // Then
            result.ShouldBeSameAs(expectedView);
        }

        [Fact]
        public void Should_return_null_when_view_cannot_be_located_using_name_and_extension()
        {
            // Given
            var expectedView = new ViewLocationResult(string.Empty, "index", "spark", () => null);

            var locator = CreateViewLocator(expectedView);

            // When
            var result = locator.LocateView("index.cshtml", null);

            // Then
            result.ShouldBeNull();
        }

        [Fact]
        public void Should_locate_view_when_name_extension_and_location_are_provided()
        {
            // Given
            var expectedView = new ViewLocationResult("views/sub", "index", "cshtml", () => null);

            var locator = CreateViewLocator(expectedView);

            // When
            var result = locator.LocateView("views/sub/index.cshtml", null);

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
            var locator = CreateViewLocator(expectedView);

            // When
            var result = locator.LocateView(viewName, null);

            // Then
            result.ShouldBeSameAs(expectedView);
        }

        [Fact]
        public void Should_return_null_when_view_cannot_be_located_using_name_extension_and_location()
        {
            // Given
            var expectedView = new ViewLocationResult("views/sub", "index", "spark", () => null);

            var locator = CreateViewLocator(expectedView);

            // When
            var result = locator.LocateView("views/feature/index.cshtml", null);

            // Then
            result.ShouldBeNull();
        }

        [Fact]
        public void Should_be_able_to_locate_view_by_name_when_two_views_with_same_name_exists_at_different_locations()
        {
            // Given
            var expectedView = new ViewLocationResult("views/sub", "index", string.Empty, () => null);
            var additionalView = new ViewLocationResult("views", "index", string.Empty, () => null);
            var locator = CreateViewLocator(expectedView, additionalView);

            // When
            var result = locator.LocateView("views/sub/index", null);

            // Then
            result.ShouldBeSameAs(expectedView);
        }

        [Fact]
        public void Should_be_able_to_locate_view_by_name_and_extension_when_two_view_with_same_name_but_different_extensions_exists_in_the_same_location()
        {
            // Given
            var expectedView = new ViewLocationResult("views", "index", "cshtml", () => null);
            var additionalView = new ViewLocationResult("views", "index", "spark", () => null);
            var locator = CreateViewLocator(expectedView, additionalView);

            // When
            var result = locator.LocateView("views/index.cshtml", null);

            // Then
            result.ShouldBeSameAs(expectedView);
        }

        [Fact]
        public void Should_be_able_to_locate_view_by_name_when_two_views_with_same_name_and_extension_exists_at_different_locations()
        {
            // Given
            var expectedView = new ViewLocationResult("views/sub", "index", "cshtml", () => null);
            var additionalView = new ViewLocationResult("views", "index", "spark", () => null);
            var locator = CreateViewLocator(expectedView, additionalView);

            // When
            var result = locator.LocateView("views/sub/index.cshtml", null);

            // Then
            result.ShouldBeSameAs(expectedView);
        }

        [Fact]
        public void Should_be_able_to_locate_view_by_name_when_the_viewname_occures_in_the_location()
        {
           // Given
           var expectedView = new ViewLocationResult( "views/hello", "hello", "cshtml", () => null );
           //var additionalView = new ViewLocationResult( "views", "index", "spark", () => null );
           var locator = CreateViewLocator(expectedView);

           // When
           var result = locator.LocateView( "views/hello/hello", null );

           // Then
           result.ShouldBeSameAs( expectedView );
        }

        [Fact]
        public void Should_get_located_views_from_view_location_providers_with_available_extensions_when_created()
        {
            // Given
            var viewEngine1 = A.Fake<IViewEngine>();
            A.CallTo(() => viewEngine1.Extensions).Returns(new[] { "html" });

            var viewEngine2 = A.Fake<IViewEngine>();
            A.CallTo(() => viewEngine2.Extensions).Returns(new[] { "spark" });

            var viewLocationProvider = A.Fake<IViewLocationProvider>();
            var expectedViewEngineExtensions = new[] { "html", "spark" };
            var viewEngine = A.Fake<IViewEngine>();
            A.CallTo(() => viewEngine.Extensions).Returns(expectedViewEngineExtensions);

            // When
            new DefaultViewLocator(viewLocationProvider, new[] { viewEngine });

            // Then
            A.CallTo(() => viewLocationProvider.GetLocatedViews(A<IEnumerable<string>>.That.Matches(
                    x => x.All(expectedViewEngineExtensions.Contains)))).MustHaveHappened();
        }


        private static DefaultViewLocator CreateViewLocator(params ViewLocationResult[] results)
        {
            var viewLocationProvider = A.Fake<IViewLocationProvider>();
            A.CallTo(() => viewLocationProvider.GetLocatedViews(A<IEnumerable<string>>._))
                                               .Returns(results);

            var viewEngine = A.Fake<IViewEngine>();
            A.CallTo(() => viewEngine.Extensions).Returns(new[] { "liquid" });

            var viewLocator = new DefaultViewLocator(viewLocationProvider, new[] { viewEngine });

            return viewLocator;
        }
    }
}