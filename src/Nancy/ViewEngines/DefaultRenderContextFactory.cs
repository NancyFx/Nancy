namespace Nancy.ViewEngines
{
    using Nancy.Localization;

    /// <summary>
    /// Default render context factory implementation.
    /// </summary>
    public class DefaultRenderContextFactory : IRenderContextFactory
    {
        private readonly IViewCache viewCache;
        private readonly IViewResolver viewResolver;
        private readonly ITextResource textResource;

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultRenderContextFactory"/> class.
        /// </summary>
        /// <param name="viewCache">The view cache that should be used by the created render context.</param>
        /// <param name="viewResolver">The view resolver that should be used by the created render context.</param>
        /// <param name="textResource">The <see cref="ITextResource"/> that should be used by the engine.</param>
        public DefaultRenderContextFactory(IViewCache viewCache, IViewResolver viewResolver, ITextResource textResource)
        {
            this.viewCache = viewCache;
            this.viewResolver = viewResolver;
            this.textResource = textResource;
        }

        /// <summary>
        /// Gets a <see cref="IRenderContext"/> for the specified <see cref="ViewLocationContext"/>.
        /// </summary>
        /// <param name="viewLocationContext">The <see cref="ViewLocationContext"/> for which the context should be created.</param>
        /// <returns>A <see cref="IRenderContext"/> instance.</returns>
        public IRenderContext GetRenderContext(ViewLocationContext viewLocationContext)
        {
            return new DefaultRenderContext(this.viewResolver, this.viewCache, this.textResource, viewLocationContext);
        }
    }
}