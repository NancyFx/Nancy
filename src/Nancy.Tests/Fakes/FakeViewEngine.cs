namespace Nancy.Tests.Fakes
{
    using ViewEngines;

    public class FakeViewEngine : IViewEngine
    {
        public ViewResult RenderView<TModel>(string viewTemplate, TModel model)
        {
            return new ViewResult(null, "fake");
        }
    }
}