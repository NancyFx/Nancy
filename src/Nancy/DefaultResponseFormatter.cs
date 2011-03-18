using System;
using Nancy.MimeType;

namespace Nancy
{
    /// <summary>
    /// The default implementation of the <see cref="IResponseFormatter"/> interface.
    /// </summary>
    public class DefaultResponseFormatter : IResponseFormatter
    {
        private readonly IRootPathProvider rootPathProvider;
        private readonly IMimeTypeRegistry mimeTypeRegistry;

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultResponseFormatter"/> class.
        /// </summary>
        /// <param name="rootPathProvider">The <see cref="IRootPathProvider"/> that should be used by the instance.</param>
        /// <param name="mimeTypeRegistry">The <see cref="IMimeTypeRegistry"/> that should be used by the instance.</param>
        public DefaultResponseFormatter(IRootPathProvider rootPathProvider, IMimeTypeRegistry mimeTypeRegistry)
        {
            this.rootPathProvider = rootPathProvider;
            this.mimeTypeRegistry = mimeTypeRegistry;
        }

        /// <summary>
        /// Gets root path of the Nancy application.
        /// </summary>
        /// <value>A <see cref="string"/> containing the root path.</value>
        public string RootPath
        {
            get { return this.rootPathProvider.GetRootPath(); }
        }

        /// <summary>
        /// Gets the mime-type registry
        /// </summary>                
        public IMimeTypeRegistry MimeTypeRegistry
        {
            get { return this.mimeTypeRegistry; }
        }
    }
}