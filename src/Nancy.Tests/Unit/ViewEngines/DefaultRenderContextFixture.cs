namespace Nancy.Tests.Unit.ViewEngines
{
    using FakeItEasy;
    using Nancy.ViewEngines;
    using Xunit;

    public class DefaultRenderContextFixture
    {
        [Fact]
        public void Should_html_encode_string()
        {
            // Given
            var context = new DefaultRenderContext(null, null, null);

            // When
            var result = context.HtmlEncode("This is a string value & should be HTML-encoded");

            // Then
            result.ShouldEqual("This is a string value &amp; should be HTML-encoded");
        }

        [Fact]
        public void Should_expose_view_cache_instance_that_is_passed_in()
        {
            // Given
            var cache = A.Fake<IViewCache>();

            // When
            var context = new DefaultRenderContext(null, cache, null);

            // Then
            context.ViewCache.ShouldBeSameAs(cache);
        }

        [Fact]
        public void Should_call_view_resolver_with_view_name_when_locating_view()
        {
            // Given
            const string viewName = "view.html";
            var resolver = A.Fake<IViewResolver>();
            var context = new DefaultRenderContext(resolver, null, null);

            // When
            context.LocateView(viewName, null);

            // Then
            A.CallTo(() => resolver.GetViewLocation(viewName, A<object>.Ignored, A<ViewLocationContext>.Ignored)).MustHaveHappened();
        }

        [Fact]
        public void Should_call_view_resolver_with_model_when_locating_view()
        {
            // Given
            var model = new object();
            var resolver = A.Fake<IViewResolver>();
            var context = new DefaultRenderContext(resolver, null, null);

            // When
            context.LocateView(null, model);

            // Then
            A.CallTo(() => resolver.GetViewLocation(A<string>.Ignored, model, A<ViewLocationContext>.Ignored)).MustHaveHappened();
        }

        [Fact]
        public void Should_call_view_resolver_with_view_location_context_when_locating_view()
        {
            // Given
            var locationContext = new ViewLocationContext();
            var resolver = A.Fake<IViewResolver>();
            var context = new DefaultRenderContext(resolver, null, locationContext);

            // When
            context.LocateView(null, null);

            // Then)
            A.CallTo(() => resolver.GetViewLocation(A<string>.Ignored, A<object>.Ignored, locationContext)).MustHaveHappened();
        }

        [Fact]
        public void Should_return_result_from_view_resolver_when_locating_view()
        {
            // Given
            var viewResult = new ViewLocationResult(null, null, null, null);
            var resolver = A.Fake<IViewResolver>();
            A.CallTo(() => resolver.GetViewLocation(A<string>.Ignored, A<object>.Ignored, A<ViewLocationContext>.Ignored)).Returns(viewResult);
            var context = new DefaultRenderContext(resolver, null, null);

            // When
            var result = context.LocateView(null, null);

            // Then
            result.ShouldBeSameAs(viewResult);
        }
    }
}