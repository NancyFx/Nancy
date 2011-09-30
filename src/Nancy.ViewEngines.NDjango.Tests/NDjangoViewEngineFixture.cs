namespace Nancy.ViewEngines.NDjango.Tests
{
    using System;
    using System.IO;
    using FakeItEasy;
    using global::NDjango.Interfaces;
    using Nancy.Tests;
    using Xunit;

    public class NDjangoViewEngineFixture
    {
        private readonly NDjangoViewEngine engine;
        private readonly IRenderContext renderContext;

        public NDjangoViewEngineFixture()
        {
            this.engine = new NDjangoViewEngine();

            var cache = A.Fake<IViewCache>();
            A.CallTo(() => cache.GetOrAdd(A<ViewLocationResult>.Ignored, A<Func<ViewLocationResult, string>>.Ignored))
                .ReturnsLazily(x =>
                {
                    var result = x.GetArgument<ViewLocationResult>(0);
                    return x.GetArgument<Func<ViewLocationResult, string>>(1).Invoke(result);
                });

            this.renderContext = A.Fake<IRenderContext>();
            A.CallTo(() => this.renderContext.ViewCache).Returns(cache);
        }

        [Fact]
        public void GetCompiledView_should_render_to_stream()
        {
            // Given
            var location = new ViewLocationResult(
                string.Empty,
                string.Empty,
                "django",
                () => new StringReader(@"{% ifequal a a %}<h1>Hello Mr. test</h1>{% endifequal %}")
            );
            A.CallTo(() => this.renderContext.LocateView(".django", null)).Returns(location);

            var stream = new MemoryStream();

            // When
            var response = engine.RenderView(location, null, this.renderContext);
            response.Contents.Invoke(stream);

            // Then
            stream.ShouldEqual("<h1>Hello Mr. test</h1>");
        }
    }
}