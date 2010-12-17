namespace Nancy.Tests.Unit.ViewEngines
{
    using System.IO;
    using FakeItEasy;
    using Nancy.ViewEngines;
    using Xunit;

    public class ViewEngineFixture
    {
        private readonly IViewLocator templateLocator;
        private readonly IViewCompiler viewCompiler;
        private readonly IView view;
        private readonly ViewEngine engine;

        public ViewEngineFixture()
        {
            this.templateLocator = A.Fake<IViewLocator>();
            this.viewCompiler = A.Fake<IViewCompiler>();
            this.view = A.Fake<IView>();

            A.CallTo(() => templateLocator.GetTemplateContents("test")).Returns(@"c:\some\fake\path");
            
            A.CallTo(() => viewCompiler.GetCompiledView<object>(null)).Returns(view);

            this.engine = new ViewEngine(templateLocator, viewCompiler);
        }

        [Fact]
        public void RenderView_Should_set_path()
        {
            // Given, When
            var result = engine.RenderView<object>("test", null);

            // Then
            result.Location.ShouldEqual(@"c:\some\fake\path");
        }

        [Fact]
        public void RenderView_Should_return_compiled_view()
        {
            // Given
            var stream = new MemoryStream();

            // When
            var result = engine.RenderView("test", stream);

            // Then
            result.View.Model.ShouldBeSameAs(stream);
        }
    }
}
