namespace Nancy.ViewEngines
{
    public class StaticViewEngine : IViewEngine
    {
        private readonly IViewLocator viewTemplateLocator;

        public StaticViewEngine(IViewLocator viewTemplateLocator)
        {
            this.viewTemplateLocator = viewTemplateLocator;
        }

        public ViewResult RenderView<TModel>(string viewTemplate, TModel model)
        {
            var result = viewTemplateLocator.GetTemplateContents(viewTemplate);

            var view = new StaticView(result.Contents);
            return new ViewResult(view, result.Location);
        }
    }
}