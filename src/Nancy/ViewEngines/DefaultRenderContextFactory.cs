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
        private readonly CryptographyConfiguration cryptographyConfiguration;
        private readonly ISessionObjectFormatter formatter;

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultRenderContextFactory"/> class.
        /// </summary>
        /// <param name="viewCache">The view cache that should be used by the created render context.</param>
        /// <param name="viewResolver">The view resolver that should be sused by the created render context.</param>
        /// <param name="cryptographyConfiguration"></param>
        /// <param name="formatter"></param>
        public DefaultRenderContextFactory(IViewCache viewCache, IViewResolver viewResolver, CryptographyConfiguration cryptographyConfiguration, ISessionObjectFormatter formatter)
        {
            this.viewCache = viewCache;
            this.viewResolver = viewResolver;
            this.cryptographyConfiguration = cryptographyConfiguration;
            this.formatter = formatter;
        }

        /// <summary>
        /// Gets a <see cref="IRenderContext"/> for the specified <see cref="ViewLocationContext"/>.
        /// </summary>
        /// <param name="viewLocationContext">The <see cref="ViewLocationContext"/> for which the context should be created.</param>
        /// <returns>A <see cref="IRenderContext"/> instance.</returns>
        public IRenderContext GetRenderContext(ViewLocationContext viewLocationContext)
        {
            return new DefaultRenderContext(this.viewResolver, this.viewCache, this.cryptographyConfiguration, this.formatter, viewLocationContext);
        }
    }
}