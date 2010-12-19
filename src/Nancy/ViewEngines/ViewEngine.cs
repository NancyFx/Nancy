namespace Nancy.ViewEngines
{
    using System;

    public abstract class ViewEngine
    {
        protected ViewEngine(IViewLocator viewTemplateLocator)
        {
            ViewTemplateLocator = viewTemplateLocator;
        }

        public IViewLocator ViewTemplateLocator { get; private set; }

        public ViewResult RenderView<TModel>(string viewTemplate, TModel model)
        {
            var result = ViewTemplateLocator.GetTemplateContents(viewTemplate);

            var view = GetCompiledView<TModel>(result);

            if (view == null)
            {
                // TODO: This should be a resource string
                throw new InvalidOperationException(String.Format("Could not find a valid view at the location '{0}'", result));
            }

            view.Model = model;

            return new ViewResult(view, result.Location);
        }

        protected abstract IView GetCompiledView<TModel>(ViewLocationResult result);
    }
}