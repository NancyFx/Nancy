namespace Nancy.ViewEngines
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;

    public interface IViewFactory
    {
        Action<Stream> this[string viewName] { get; }

        Action<Stream> this[string viewName, dynamic model] { get; }
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

        public Action<Stream> this[string viewName]
        {
            get { return this.GetRenderedView(viewName, null); }
        }

        public Action<Stream> this[string viewName, dynamic model]
        {
            get { return this.GetRenderedView(viewName, model); }
        }

        private Action<Stream> GetRenderedView(string viewName, dynamic model)
        {
            if (viewName == null)
            {
                return EmptyView;
            }

            var viewLocationResult =
                this.viewLocator.GetViewLocation(Path.GetFileNameWithoutExtension(viewName), this.GetExtensionsToUseForViewLookup(viewName));

            // CHECK FOR NULL LOCATION!

            var resolvedViewEngine = 
                GetViewEngine(viewLocationResult.Extension);

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

        private IEnumerable<string> GetExtensionsToUseForViewLookup(string viewName)
        {
            var extensions =
                GetViewExtension(viewName) ?? GetSupportedViewEngineExtensions();

            return extensions;
        }

        private static IEnumerable<string> GetViewExtension(string viewName)
        {
            var extension =
                Path.GetExtension(viewName);

            return string.IsNullOrEmpty(extension) ? null : new[] {extension.TrimStart('.') };
        }

        private IEnumerable<string> GetSupportedViewEngineExtensions()
        {
            var viewEngineExtensions =
                this.viewEngines.SelectMany(x => x.Extensions);

            return viewEngineExtensions.Distinct(StringComparer.OrdinalIgnoreCase);
        }

        private IViewEngineEx GetViewEngine(string extension)
        {
            var viewEngiens = 
                from viewEngine in this.viewEngines
                where viewEngine.Extensions.Any(x => x.Equals(extension, StringComparison.InvariantCultureIgnoreCase))
                select viewEngine;

            return viewEngiens.FirstOrDefault();
        }

        private static Action<Stream> SafeInvokeViewEngine(IViewEngineEx viewEngine, ViewLocationResult locationResult, dynamic model)
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

        Action<Stream> RenderView(ViewLocationResult viewLocationResult, dynamic model);
    }
}