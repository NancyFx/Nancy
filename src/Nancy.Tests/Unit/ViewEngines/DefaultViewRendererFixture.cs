namespace Nancy.Tests.Unit.ViewEngines
{
    using FakeItEasy;

    using Nancy.ViewEngines;

    using Xunit;

    public class DefaultViewRendererFixture
    {
        private readonly IViewFactory factory;
        private readonly DefaultViewRenderer renderer;

        public DefaultViewRendererFixture()
        {
            this.factory = A.Fake<IViewFactory>();
            this.renderer = new DefaultViewRenderer(this.factory);
        }

        [Fact]
        public void Should_invoke_factory_with_view_location_context_containing_context()
        {
            // Given
            var context = A.Dummy<NancyContext>();

            // When
            this.renderer.RenderView(context, null, null);

            // Then
            A.CallTo(() => this.factory.RenderView(
                A<string>.Ignored, 
                A<object>.Ignored,
                A<ViewLocationContext>.That.Matches(x => x.Context.Equals(context)))).MustHaveHappened();
        }

        [Fact]
        public void Should_invoke_factory_with_view_name()
        {
            // Given
            const string viewName = "theview";

            // When
            this.renderer.RenderView(null, viewName, null);

            // Then
            A.CallTo(() => this.factory.RenderView(viewName, A<object>.Ignored, A<ViewLocationContext>.Ignored)).MustHaveHappened();
        }

        [Fact]
        public void Should_invoke_factory_with_model()
        {
            // Given
            var model = new object();

            // When
            this.renderer.RenderView(null, null, model);

            // Then
            A.CallTo(() => this.factory.RenderView(A<string>.Ignored, model, A<ViewLocationContext>.Ignored)).MustHaveHappened();
        }
    }
}