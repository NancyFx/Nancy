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
            A.CallTo(() => cache.GetOrAdd(A<ViewLocationResult>.Ignored, A<Func<ViewLocationResult, ITemplate>>.Ignored))
                .ReturnsLazily(x =>
                {
                    var result = x.GetArgument<ViewLocationResult>(0);
                    return x.GetArgument<Func<ViewLocationResult, ITemplate>>(1).Invoke(result);
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

            var stream = new MemoryStream();

            // When
            var action = engine.RenderView(location, null, this.renderContext);
            action.Invoke(stream);

            // Then
            stream.ShouldEqual("<h1>Hello Mr. test</h1>");
        }
    }
}