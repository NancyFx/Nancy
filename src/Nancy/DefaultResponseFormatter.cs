namespace Nancy
{
    /// <summary>
    /// The default implementation of the <see cref="IResponseFormatter"/> interface.
    /// </summary>
    public class DefaultResponseFormatter : IResponseFormatter
    {
        private readonly IRootPathProvider rootPathProvider;

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultResponseFormatter"/> class.
        /// </summary>
        /// <param name="rootPathProvider">The <see cref="IRootPathProvider"/> that should be used by the instance.</param>
        public DefaultResponseFormatter(IRootPathProvider rootPathProvider)
        {
            this.rootPathProvider = rootPathProvider;
        }

        /// <summary>
        /// Gets root path of the Nancy application.
        /// </summary>
        /// <value>A <see cref="string"/> containing the root path.</value>
        public string RootPath
        {
            get { return this.rootPathProvider.GetRootPath(); }
        }
    }
}