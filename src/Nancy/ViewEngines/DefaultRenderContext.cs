namespace Nancy.ViewEngines
{
    /// <summary>
    /// Default render context implementation.
    /// </summary>
    public class DefaultRenderContext : IRenderContext
    {
        private readonly IViewResolver viewResolver;
        private readonly IViewCache viewCache;
        private readonly ViewLocationContext viewLocationContext;

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultRenderContext"/> class.
        /// </summary>
        /// <param name="viewResolver"></param>
        /// <param name="viewCache"></param>
        /// <param name="viewLocationContext"></param>
        public DefaultRenderContext(IViewResolver viewResolver, IViewCache viewCache, ViewLocationContext viewLocationContext)
        {
            this.viewResolver = viewResolver;
            this.viewCache = viewCache;
            this.viewLocationContext = viewLocationContext;
        }

        /// <summary>
        /// HTML encodes a string.
        /// </summary>
        /// <param name="input">The string that should be HTML encoded.</param>
        /// <returns>A HTML encoded <see cref="string"/>.</returns>
        public string HtmlEncode(string input)
        {
            return Helpers.HttpUtility.HtmlEncode(input);
        }

        /// <summary>
        /// Gets the view cache that is used by Nancy.
        /// </summary>
        /// <value>An <see cref="IViewCache"/> instance.</value>
        public IViewCache ViewCache
        {
            get { return this.viewCache; }
        }

        /// <summary>
        /// Locates a view that matches the provided <paramref name="viewName"/> and <paramref name="model"/>.
        /// </summary>
        /// <param name="viewName">The name of the view that should be located.</param>
        /// <param name="model">The model that should be used when locating the view.</param>
        /// <returns>A <see cref="ViewLocationResult"/> instance if the view could be located; otherwise, <see langword="null"/>.</returns>
        public ViewLocationResult LocateView(string viewName, dynamic model)
        {
            return this.viewResolver.GetViewLocation(viewName, model, this.viewLocationContext);
        }
    }
}