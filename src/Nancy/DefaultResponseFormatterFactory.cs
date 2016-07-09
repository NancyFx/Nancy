namespace Nancy
{
    using Nancy.Configuration;

    /// <summary>
    /// The default implementation of the <see cref="IResponseFormatterFactory"/> interface.
    /// </summary>
    public class DefaultResponseFormatterFactory : IResponseFormatterFactory
    {
        private readonly IRootPathProvider rootPathProvider;
        private readonly ISerializerFactory serializerFactory;
        private readonly INancyEnvironment environment;

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultResponseFormatter"/> class.
        /// </summary>
        /// <param name="rootPathProvider">An <see cref="IRootPathProvider"/> instance.</param>
        /// <param name="serializerFactory">An <see cref="ISerializerFactory"/> instance.</param>
        /// <param name="environment">An <see cref="INancyEnvironment"/> instance.</param>
        public DefaultResponseFormatterFactory(IRootPathProvider rootPathProvider, ISerializerFactory serializerFactory, INancyEnvironment environment)
        {
            this.rootPathProvider = rootPathProvider;
            this.serializerFactory = serializerFactory;
            this.environment = environment;
        }

        /// <summary>
        /// Creates a new <see cref="IResponseFormatter"/> instance.
        /// </summary>
        /// <param name="context">The <see cref="NancyContext"/> instance that should be used by the response formatter.</param>
        /// <returns>An <see cref="IResponseFormatter"/> instance.</returns>
        public IResponseFormatter Create(NancyContext context)
        {
            return new DefaultResponseFormatter(this.rootPathProvider, context, this.serializerFactory, this.environment);
        }
    }
}