namespace Nancy
{
    using System;

    /// <summary>
    /// The default implementation of the <see cref="IResponseFormatter"/> interface.
    /// </summary>
    public class DefaultResponseFormatter : IResponseFormatter
    {
        private readonly IRootPathProvider rootPathProvider;
        private readonly NancyContext context;

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultResponseFormatter"/> class.
        /// </summary>
        /// <param name="rootPathProvider">The <see cref="IRootPathProvider"/> that should be used by the instance.</param>
        /// <param name="context">The <see cref="NancyContext"/> that should be used by the instance.</param>
        public DefaultResponseFormatter(IRootPathProvider rootPathProvider, NancyContext context)
        {
            this.rootPathProvider = rootPathProvider;
            this.context = context;
        }

        /// <summary>
        /// Gets the context for which the response is being formatted.
        /// </summary>
        /// <value>A <see cref="NancyContext"/> intance.</value>
        public NancyContext Context
        {
            get { return this.context; }
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