namespace Nancy
{
    using Nancy.Configuration;

    /// <summary>
    /// The default implementation of the <see cref="IResponseFormatter"/> interface.
    /// </summary>
    public class DefaultResponseFormatter : IResponseFormatter
    {
        private readonly IRootPathProvider rootPathProvider;
        private readonly NancyContext context;
        private readonly ISerializerFactory serializerFactory;
        private readonly INancyEnvironment environment;

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultResponseFormatter"/> class.
        /// </summary>
        /// <param name="rootPathProvider">The <see cref="IRootPathProvider"/> that should be used by the instance.</param>
        /// <param name="context">The <see cref="NancyContext"/> that should be used by the instance.</param>
        /// <param name="serializerFactory">An <see cref="ISerializerFactory" /> instance"/>.</param>
        /// <param name="environment">An <see cref="INancyEnvironment"/> instance.</param>
        public DefaultResponseFormatter(IRootPathProvider rootPathProvider, NancyContext context, ISerializerFactory serializerFactory, INancyEnvironment environment)
        {
            this.rootPathProvider = rootPathProvider;
            this.context = context;
            this.serializerFactory = serializerFactory;
            this.environment = environment;
        }

        /// <summary>
        /// Gets all <see cref="ISerializerFactory"/> factory.
        /// </summary>
        public ISerializerFactory SerializerFactory
        {
            get { return this.serializerFactory; }
        }

        /// <summary>
        /// Gets the context for which the response is being formatted.
        /// </summary>
        /// <value>A <see cref="NancyContext"/> instance.</value>
        public NancyContext Context
        {
            get { return this.context; }
        }

        /// <summary>
        /// Gets the <see cref="INancyEnvironment"/>.
        /// </summary>
        /// <value>An <see cref="INancyEnvironment"/> instance.</value>
        public INancyEnvironment Environment
        {
            get { return this.environment; }
        }

        /// <summary>
        /// Gets the root path of the application.
        /// </summary>
        /// <value>A <see cref="string"/> containing the root path.</value>
        public string RootPath
        {
            get { return this.rootPathProvider.GetRootPath(); }
        }
    }
}