namespace Nancy.ViewEngines
{
    /// <summary>
    /// Default implementation of the <see cref="IViewRenderer"/> interface.
    /// </summary>
    public class DefaultViewRenderer : IViewRenderer
    {
        private readonly IViewFactory factory;

        /// <summary>
        /// Initializes an instance of the <see cref="DefaultViewRenderer"/> type, with
        /// the provided <paramref name="factory"/>.
        /// </summary>
        /// <param name="factory">The <see cref="IViewFactory"/> that should be used to render the views.</param>
        public DefaultViewRenderer(IViewFactory factory)
        {
            this.factory = factory;
        }

        /// <summary>
        /// Renders a view to a response object, bypassing content negotiation.
        /// </summary>
        /// <param name="context">Current Nancy context</param>
        /// <param name="viewName">View name</param>
        /// <param name="model">Model object (or null)</param>
        /// <returns>Response object containing the rendered view (if found)</returns>
        public Response RenderView(NancyContext context, string viewName, object model = null)
        {
            var viewContext = new ViewLocationContext { Context = context };

            return this.factory.RenderView(viewName, model, viewContext);
        }
    }
}