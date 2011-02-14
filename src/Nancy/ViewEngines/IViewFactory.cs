namespace Nancy.ViewEngines
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;

    public interface IViewFactory
    {
        Action<Stream> GetRenderedView<TModel>(string viewName, TModel model);
    }

    public class DefaultViewFactory : IViewFactory
    {
        private readonly IViewLocator viewLocator;
        private readonly IEnumerable<IViewEngineEx> viewEngines;
        private static readonly Action<Stream> EmptyView = x => { };

        public DefaultViewFactory(IViewLocator viewLocator, IEnumerable<IViewEngineEx> viewEngines)
        {
            this.viewLocator = viewLocator;
            this.viewEngines = viewEngines;
        }

        public Action<Stream> GetRenderedView<TModel>(string viewName, TModel model)
        {
            if (viewName == null)
            {
                return EmptyView;
            }

            var viewLocationResult =
                this.viewLocator.GetViewLocation(viewName);

            var resolvedViewEngine = GetViewEngine(viewName);

            if (resolvedViewEngine == null)
            {
                return EmptyView;
            }

            return SafeInvokeViewEngine(
                resolvedViewEngine,
                viewLocationResult,
                model
            );
        }

        private IViewEngineEx GetViewEngine(string viewName)
        {
            var viewExtension =
                Path.GetExtension(viewName).TrimStart('.');

            var viewEngiens = 
                from viewEngine in this.viewEngines
                where viewEngine.Extensions.Any(x => x.Equals(viewExtension, StringComparison.InvariantCultureIgnoreCase))
                select viewEngine;

            return viewEngiens.FirstOrDefault();
        }

        private static Action<Stream> SafeInvokeViewEngine<TModel>(IViewEngineEx viewEngine, ViewLocationResult locationResult, TModel model)
        {
            try
            {
                return viewEngine.RenderView(locationResult, model);
            }
            catch (Exception)
            {
                return EmptyView;
            }
        }
    }

    public interface IViewEngineEx
    {
        IEnumerable<string> Extensions { get; }

        Action<Stream> RenderView<TModel>(ViewLocationResult viewLocationResult, TModel model);
    }
}