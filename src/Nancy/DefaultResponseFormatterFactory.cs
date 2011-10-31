namespace Nancy
{
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// The default implementation of the <see cref="IResponseFormatterFactory"/> interface.
    /// </summary>
    public class DefaultResponseFormatterFactory : IResponseFormatterFactory
    {
        private readonly IRootPathProvider rootPathProvider;

        private readonly IEnumerable<ISerializer> serializers;

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultResponseFormatter"/> class, with the
        /// provided <see cref="IRootPathProvider"/>.
        /// </summary>
        /// <param name="rootPathProvider"></param>
        public DefaultResponseFormatterFactory(IRootPathProvider rootPathProvider, IEnumerable<ISerializer> serializers)
        {
            this.rootPathProvider = rootPathProvider;
            this.serializers = serializers.ToArray();
        }

        /// <summary>
        /// Creates a new <see cref="IResponseFormatter"/> instance.
        /// </summary>
        /// <param name="context">The <see cref="NancyContext"/> instance that should be used by the response formatter.</param>
        /// <returns>An <see cref="IResponseFormatter"/> instance.</returns>
        public IResponseFormatter Create(NancyContext context)
        {
            return new DefaultResponseFormatter(this.rootPathProvider, context, this.serializers);
        }
    }
}