namespace Nancy.ViewEngines.NDjango
{
    using System.Collections.Generic;
    using System.IO;
    using global::NDjango;

    public class NDjangoViewEngine
    {
        public NDjangoViewEngine() : this(new AspNetTemplateLocator())
        {
        }

        public NDjangoViewEngine(IViewLocator viewTemplateLocator)
        {
            ViewTemplateLocator = viewTemplateLocator;
        }

        public IViewLocator ViewTemplateLocator { get; private set; }

        public ViewResult RenderView<TModel>(string viewTemplate, TModel model)
        {
            var templateManagerProvider = new TemplateManagerProvider();
            var manager = templateManagerProvider.GetNewManager();

            var result = ViewTemplateLocator.GetTemplateContents(viewTemplate);

            var context = new Dictionary<string, object> { { "Model", model } };

            string location = result.Location;

            TextReader reader = manager.RenderTemplate(location, context);

            var view = new NDjangoView(reader) {Model = model};

            return new ViewResult(view, location);
        }       
    }
}