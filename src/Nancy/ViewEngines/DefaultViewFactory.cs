namespace Nancy.ViewEngines
{
    using System;
    using System.Collections.Generic;
    using System.Dynamic;
    using System.IO;
    using System.Linq;
    using System.Text.RegularExpressions;

    using Nancy.Conventions;

    /// <summary>
    /// The default implementation for how views are resolved and rendered by Nancy.
    /// </summary>
    public class DefaultViewFactory : IViewFactory
    {
        private readonly IViewResolver viewResolver;
        private readonly IEnumerable<IViewEngine> viewEngines;
        private readonly IRenderContextFactory renderContextFactory;
        private readonly ViewLocationConventions conventions;
        private readonly IRootPathProvider rootPathProvider;
        private static readonly Action<Stream> EmptyView = x => { };
        private readonly string[] viewEngineExtensions;

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultViewFactory"/> class.
        /// </summary>
        /// <param name="viewResolver">An <see cref="IViewResolver"/> instance that should be used to resolve the location of a view.</param>
        /// <param name="viewEngines">An <see cref="IEnumerable{T}"/> instance containing the <see cref="IViewEngine"/> instances that should be able to be used to render a view</param>
        /// <param name="renderContextFactory">A <see cref="IRenderContextFactory"/> instance that should be used to create an <see cref="IRenderContext"/> when a view is rendered.</param>
        /// <param name="conventions">An <see cref="ViewLocationConventions"/> instance that should be used to resolve all possible view locations </param>
        /// <param name="rootPathProvider">An <see cref="IRootPathProvider"/> instance.</param>
        public DefaultViewFactory(IViewResolver viewResolver, IEnumerable<IViewEngine> viewEngines, IRenderContextFactory renderContextFactory, ViewLocationConventions conventions, IRootPathProvider rootPathProvider)
        {
            this.viewResolver = viewResolver;
            this.viewEngines = viewEngines;
            this.renderContextFactory = renderContextFactory;
            this.conventions = conventions;
            this.rootPathProvider = rootPathProvider;

            this.viewEngineExtensions = this.viewEngines.SelectMany(ive => ive.Extensions).ToArray();
        }

        /// <summary>
        /// Renders the view with the name and model defined by the <paramref name="viewName"/> and <paramref name="model"/> parameters.
        /// </summary>
        /// <param name="viewName">The name of the view to render.</param>
        /// <param name="model">The model that should be passed into the view.</param>
        /// <param name="viewLocationContext">A <see cref="ViewLocationContext"/> instance, containing information about the context for which the view is being rendered.</param>
        /// <returns>A delegate that can be invoked with the <see cref="Stream"/> that the view should be rendered to.</returns>
        public Response RenderView(string viewName, dynamic model, ViewLocationContext viewLocationContext)
        {
            if (viewName == null && model == null)
            {
                throw new ArgumentException("View name and model parameters cannot both be null.");
            }

            if (model == null && viewName.Length == 0)
            {
                throw new ArgumentException("The view name parameter cannot be empty when the model parameters is null.");
            }

            if (viewLocationContext == null)
            {
                throw new ArgumentNullException("viewLocationContext", "The value of the viewLocationContext parameter cannot be null.");
            }

            var actualViewName =
                viewName ?? GetViewNameFromModel(model, viewLocationContext.Context);

            viewLocationContext.Context.Trace.TraceLog.WriteLog(x => x.AppendLine(string.Concat("[DefaultViewFactory] Rendering view with name ", actualViewName)));

            return this.GetRenderedView(actualViewName, model, viewLocationContext);
        }

        private Response GetRenderedView(string viewName, dynamic model, ViewLocationContext viewLocationContext)
        {
            var viewLocationResult =
                this.viewResolver.GetViewLocation(viewName, model, viewLocationContext);

            var resolvedViewEngine =
                GetViewEngine(viewLocationResult, viewLocationContext.Context);

            if (resolvedViewEngine == null)
            {
                viewLocationContext.Context.Trace.TraceLog.WriteLog(x => x.AppendLine("[DefaultViewFactory] Unable to find view engine that could render the view."));
                throw new ViewNotFoundException(viewName, this.viewEngineExtensions, this.GetInspectedLocations(viewName, model, viewLocationContext), this.rootPathProvider);
            }

            viewLocationContext.Context.Trace.TraceLog.WriteLog(x => x.AppendLine(string.Concat("[DefaultViewFactory] Rendering view with view engine ", resolvedViewEngine.GetType().FullName)));

            return SafeInvokeViewEngine(
                resolvedViewEngine,
                viewLocationResult,
                GetSafeModel(model),
                this.renderContextFactory.GetRenderContext(viewLocationContext)
            );
        }

        private string[] GetInspectedLocations(string viewName, dynamic model, ViewLocationContext viewLocationContext)
        {
            var inspectedLocations = new List<string>();

            foreach (var convention in conventions)
            {
                try
                {
                    var location =
                        convention.Invoke(viewName, model, viewLocationContext);

                    if (!string.IsNullOrWhiteSpace(location))
                    {
                        inspectedLocations.Add(location);
                    }
                }
                catch
                {
                }
            }

            return inspectedLocations.ToArray();
        }

        private static object GetSafeModel(object model)
        {
            return (model.IsAnonymousType()) ? GetExpandoObject(model) : model;
        }

        private static ExpandoObject GetExpandoObject(object source)
        {
            var expandoObject = new ExpandoObject();
            IDictionary<string, object> results = expandoObject;

            foreach (var propertyInfo in source.GetType().GetProperties())
            {
                results[propertyInfo.Name] = propertyInfo.GetValue(source, null);
            }

            return expandoObject;
        }

        private IViewEngine GetViewEngine(ViewLocationResult viewLocationResult, NancyContext context)
        {
            if (viewLocationResult == null)
            {
                return null;
            }

            context.Trace.TraceLog.WriteLog(x => x.AppendLine(string.Concat("[DefaultViewFactory] Attempting to resolve view engine for view extension ", viewLocationResult.Extension)));

            var matchingViewEngines =
                from viewEngine in this.viewEngines
                where viewEngine.Extensions.Any(x => x.Equals(viewLocationResult.Extension, StringComparison.OrdinalIgnoreCase))
                select viewEngine;

            return matchingViewEngines.FirstOrDefault();
        }

        private static string GetViewNameFromModel(dynamic model, NancyContext context)
        {
            context.Trace.TraceLog.WriteLog(x => x.AppendLine(string.Concat("[DefaultViewFactory] Extracting view name from model of type ", model.GetType().FullName)));

            return Regex.Replace(model.GetType().Name, "Model$", string.Empty);
        }

        private static Response SafeInvokeViewEngine(IViewEngine viewEngine, ViewLocationResult locationResult, dynamic model, IRenderContext renderContext)
        {
            try
            {
                return viewEngine.RenderView(locationResult, model, renderContext);
            }
            catch (Exception)
            {
                return EmptyView;
            }
        }
    }
}