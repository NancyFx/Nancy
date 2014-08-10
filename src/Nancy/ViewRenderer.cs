namespace Nancy
{
    using System.IO;

    using Nancy.Responses.Negotiation;

    /// <summary>
    /// Helper class for rendering a view from a route handler.
    /// </summary>
    public class ViewRenderer : IHideObjectMembers
    {
        private readonly INancyModule module;

        /// <summary>
        /// Initializes a new instance of the <see cref="ViewRenderer"/> class.
        /// </summary>
        /// <param name="module">The <see cref="INancyModule"/> instance that is rendering the view.</param>
        public ViewRenderer(INancyModule module)
        {
            this.module = module;
        }

        /// <summary>
        /// Renders the view with its name resolved from the model type, and model defined by the <paramref name="model"/> parameter.
        /// </summary>
        /// <param name="model">The model that should be passed into the view.</param>
        /// <returns>A delegate that can be invoked with the <see cref="Stream"/> that the view should be rendered to.</returns>
        /// <remarks>The view name is model.GetType().Name with any Model suffix removed.</remarks>
        public Negotiator this[dynamic model]
        {
            get { return this.GetNegotiator(null, model); }
        }

        /// <summary>
        /// Renders the view with the name defined by the <paramref name="viewName"/> parameter.
        /// </summary>
        /// <param name="viewName">The name of the view to render.</param>
        /// <returns>A delegate that can be invoked with the <see cref="Stream"/> that the view should be rendered to.</returns>
        /// <remarks>The extension in the view name is optional. If it is omitted, then Nancy will try to resolve which of the available engines that should be used to render the view.</remarks>
        public Negotiator this[string viewName]
        {
            get { return this.GetNegotiator(viewName, null); }
        }

        /// <summary>
        /// Renders the view with the name and model defined by the <paramref name="viewName"/> and <paramref name="model"/> parameters.
        /// </summary>
        /// <param name="viewName">The name of the view to render.</param>
        /// <param name="model">The model that should be passed into the view.</param>
        /// <returns>A delegate that can be invoked with the <see cref="Stream"/> that the view should be rendered to.</returns>
        /// <remarks>The extension in the view name is optional. If it is omitted, then Nancy will try to resolve which of the available engines that should be used to render the view.</remarks>
        public Negotiator this[string viewName, dynamic model]
        {
            get { return this.GetNegotiator(viewName, model); }
        }

        private Negotiator GetNegotiator(string viewName, object model)
        {
            var negotiationContext = this.module.Context.NegotiationContext;

            negotiationContext.ViewName = viewName;
            negotiationContext.DefaultModel = model;
            negotiationContext.PermissableMediaRanges.Clear();
            negotiationContext.PermissableMediaRanges.Add("text/html");

            return new Negotiator(this.module.Context);
        }
    }
}