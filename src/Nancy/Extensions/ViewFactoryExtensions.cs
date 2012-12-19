
namespace Nancy.Extensions
{
    using System;
    using System.Linq;
    using Nancy.ViewEngines;

    public static class ViewFactoryExtensions
    {
        /// <summary>
        /// Render View
        /// </summary>
        /// <param name="factory">The view factory.</param>
        /// <param name="cache">The view location cache.</param>
        /// <param name="context">The nancy context.</param>
        /// <param name="viewName">The view namce.</param>
        /// <param name="model">The model.</param>
        /// <returns>The response.</returns>
        public static Response RenderView(this IViewFactory factory, IViewLocationCache cache, NancyContext context, string viewName, object model = null)
        {
            var foundMatchingView = cache.Any(x => viewName.Equals(string.Concat(x.Location, "/", x.Name, ".", x.Extension), StringComparison.OrdinalIgnoreCase));

            if (foundMatchingView)
            {
                var viewContext = new ViewLocationContext { Context = context };
                context.Response = factory.RenderView(viewName, model, viewContext);
            }

            return context.Response;
        }
    }
}
