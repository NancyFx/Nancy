namespace Nancy.ViewEngines
{
    using Cryptography;
    using Session;

    /// <summary>
    /// Default render context factory implementation.
    /// </summary>
    public class DefaultRenderContextFactory : IRenderContextFactory
    {
        private readonly IViewCache viewCache;
        private readonly IViewResolver viewResolver;
        private readonly IHmacProvider hmacProvider;
        private readonly ISessionObjectFormatter formatter;

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultRenderContextFactory"/> class.
        /// </summary>
        /// <param name="viewCache">The view cache that should be used by the created render context.</param>
        /// <param name="viewResolver">The view resolver that should be sused by the created render context.</param>
        /// <param name="hmacProvider"></param>
        /// <param name="formatter"></param>
        public DefaultRenderContextFactory(IViewCache viewCache, IViewResolver viewResolver, IHmacProvider hmacProvider, ISessionObjectFormatter formatter)
        {
            this.viewCache = viewCache;
            this.viewResolver = viewResolver;
            this.hmacProvider = hmacProvider;
            this.formatter = formatter;
        }

        /// <summary>
        /// Gets a <see cref="IRenderContext"/> for the specified <see cref="ViewLocationContext"/>.
        /// </summary>
        /// <param name="viewLocationContext">The <see cref="ViewLocationContext"/> for which the context should be created.</param>
        /// <returns>A <see cref="IRenderContext"/> instance.</returns>
        public IRenderContext GetRenderContext(ViewLocationContext viewLocationContext)
        {
            return new DefaultRenderContext(this.viewResolver, this.viewCache, this.hmacProvider, this.formatter, viewLocationContext);
        }
    }
}