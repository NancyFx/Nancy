namespace Nancy.ViewEngines
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;

    /// <summary>
    /// The default implementation for how views are resolved and rendered by Nancy.
    /// </summary>
    public class DefaultViewFactory : IViewFactory
    {
        private readonly IViewLocator viewLocator;
        private readonly IEnumerable<IViewEngine> viewEngines;
        private static readonly Action<Stream> EmptyView = x => { };

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultViewFactory"/> class.
        /// </summary>
        /// <param name="viewLocator">An <see cref="IViewLocator"/> instance that should be used to resolve the location of a view.</param>
        /// <param name="viewEngines">An <see cref="IEnumerable{T}"/> instance containing the <see cref="IViewEngine"/> instances that should be able to be used to render a view</param>
        public DefaultViewFactory(IViewLocator viewLocator, IEnumerable<IViewEngine> viewEngines)
        {
            this.viewLocator = viewLocator;
            this.viewEngines = viewEngines;
        }

        /// <summary>
        /// Renders the view with the name defined by the <paramref name="viewName"/> parameter.
        /// </summary>
        /// <param name="viewName">The name of the view to render.</param>
        /// <returns>A delegate that can be invoked with the <see cref="Stream"/> that the view should be rendered to.</returns>
        /// <remarks>The extension in the view name is optional. If it is omitted, then Nancy will try to resolve which of the available engines that should be used to render the view.</remarks>
        public Action<Stream> this[string viewName]
        {
            get { return this.GetRenderedView(viewName, null); }
        }

        /// <summary>
        /// Renders the view with the name and model defined by the <paramref name="viewName"/> and <paramref name="model"/> parameters.
        /// </summary>
        /// <param name="viewName">The name of the view to render.</param>
        /// <param name="model">The model that should be passed into the view.</param>
        /// <returns>A delegate that can be invoked with the <see cref="Stream"/> that the view should be rendered to.</returns>
        /// <remarks>The extension in the view name is optional. If it is omitted, then Nancy will try to resolve which of the available engines that should be used to render the view.</remarks>
        public Action<Stream> this[string viewName, dynamic model]
        {
            get { return this.GetRenderedView(viewName, model); }
        }

        private IEnumerable<string> GetExtensionsToUseForViewLookup(string viewName)
        {
            var extensions =
                GetViewExtension(viewName) ?? GetSupportedViewEngineExtensions();

            return extensions;
        }

        private Action<Stream> GetRenderedView(string viewName, dynamic model)
        {
            var viewLocationResult =
                this.viewLocator.GetViewLocation(Path.GetFileNameWithoutExtension(viewName), this.GetExtensionsToUseForViewLookup(viewName));

            var resolvedViewEngine = 
                GetViewEngine(viewLocationResult);

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

        private IEnumerable<string> GetSupportedViewEngineExtensions()
        {
            var viewEngineExtensions =
                this.viewEngines.SelectMany(x => x.Extensions);

            return viewEngineExtensions.Distinct(StringComparer.OrdinalIgnoreCase);
        }

        private IViewEngine GetViewEngine(ViewLocationResult viewLocationResult)
        {
            if (viewLocationResult == null)
            {
                return null;
            }

            var viewEngiens = 
                from viewEngine in this.viewEngines
                where viewEngine.Extensions.Any(x => x.Equals(viewLocationResult.Extension, StringComparison.InvariantCultureIgnoreCase))
                select viewEngine;

            return viewEngiens.FirstOrDefault();
        }

        private static IEnumerable<string> GetViewExtension(string viewName)
        {
            var extension =
                Path.GetExtension(viewName);

            return string.IsNullOrEmpty(extension) ? null : new[] {extension.TrimStart('.') };
        }

        private static Action<Stream> SafeInvokeViewEngine(IViewEngine viewEngine, ViewLocationResult locationResult, dynamic model)
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
}