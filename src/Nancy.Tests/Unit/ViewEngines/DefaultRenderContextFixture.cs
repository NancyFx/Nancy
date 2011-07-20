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

        [Fact]
        public void Should_return_same_path_when_parsing_path_if_path_doesnt_contain_tilde()
        {
            const string input = "/scripts/test.js";
            var url = new Url
                {
                    BasePath = "/base/path",
                    Path = "/"
                };
            var request = new Request("GET", url);
            var nancyContext = new NancyContext { Request = request };
            var viewLocationContext = new ViewLocationContext { Context = nancyContext };
            var context = new DefaultRenderContext(null, null, viewLocationContext);

            var result = context.ParsePath(input);

            result.ShouldEqual(input);
        }

        [Fact]
        public void Should_replace_tilde_with_base_path_when_parsing_path_if_one_present()
        {
            const string input = "~/scripts/test.js";
            var url = new Url
                {
                    BasePath = "/base/path/",
                    Path = "/"
                };
            var request = new Request("GET", url);
            var nancyContext = new NancyContext { Request = request };
            var viewLocationContext = new ViewLocationContext { Context = nancyContext };
            var context = new DefaultRenderContext(null, null, viewLocationContext);

            var result = context.ParsePath(input);

            result.ShouldEqual("/base/path/scripts/test.js");
        }

        [Fact]
        public void Should_replace_tilde_with_nothing_when_parsing_path_if_one_present_and_base_path_is_null()
        {
            const string input = "~/scripts/test.js";
            var url = new Url
                {
                    BasePath = null,
                    Path = "/"
                };
            var request = new Request("GET", url);
            var nancyContext = new NancyContext { Request = request };
            var viewLocationContext = new ViewLocationContext { Context = nancyContext };
            var context = new DefaultRenderContext(null, null, viewLocationContext);

            var result = context.ParsePath(input);

            result.ShouldEqual("/scripts/test.js");
        }

    }
}