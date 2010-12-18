namespace Nancy.ViewEngines
{
    using System;

    public class ViewEngine
    {
        public ViewEngine(IViewLocator viewTemplateLocator, IViewCompiler viewCompiler)
        {
            ViewTemplateLocator = viewTemplateLocator;
            ViewCompiler = viewCompiler;
        }

        public IViewCompiler ViewCompiler { get; private set; }

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

        private IView GetCompiledView<TModel>(ViewLocationResult result)
        {
            if (ViewCompiler is IViewCompilerWithTextReaderSupport)
            {
                return (ViewCompiler as IViewCompilerWithTextReaderSupport).GetCompiledView<TModel>(result.Contents);
            }
            return ViewCompiler.GetCompiledView<TModel>(result.Location);
        }
    }
}