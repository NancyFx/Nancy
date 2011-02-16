namespace Nancy.ViewEngines
{
    using System;
    using System.Linq;

    public class ViewEngine : IViewEngine
    {
        public ViewEngine(IViewLocator viewTemplateLocator, IViewCompiler viewCompiler)
        {
            ViewTemplateLocator = viewTemplateLocator;
            ViewCompiler = viewCompiler;
        }

        public IViewLocator ViewTemplateLocator { get; private set; }
        public IViewCompiler ViewCompiler { get; private set; }

        public ViewResult RenderView<TModel>(string viewTemplate, TModel model)
        {
            var result = ViewTemplateLocator.GetViewLocation(viewTemplate, Enumerable.Empty<string>());

            var view = ViewCompiler.GetCompiledView<TModel>(result.Contents);

            if (view == null)
            {
                // TODO: This should be a resource string
                throw new InvalidOperationException(String.Format("Could not find a valid view at the location '{0}'", result));
            }

            view.Model = model;

            return new ViewResult(view, result.Location);
        }
    }
}