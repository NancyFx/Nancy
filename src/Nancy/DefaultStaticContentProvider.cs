namespace Nancy
{
    using System.Linq;

    using Nancy.Conventions;

    /// <summary>
    /// The default static content provider that uses <see cref="Nancy.Conventions.StaticContentConventions"/>
    /// to determine what static content to serve.
    /// </summary>
    public class DefaultStaticContentProvider : IStaticContentProvider
    {
        private readonly IRootPathProvider rootPathProvider;
        private readonly StaticContentsConventions conventions;

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultStaticContentProvider"/> class, using the
        /// provided <paramref name="rootPathProvider"/> and <paramref name="conventions"/>.
        /// </summary>
        /// <param name="rootPathProvider">The current root path provider.</param>
        /// <param name="conventions">The static content conventions.</param>
        public DefaultStaticContentProvider(IRootPathProvider rootPathProvider, StaticContentsConventions conventions)
        {
            this.rootPathProvider = rootPathProvider;
            this.conventions = conventions;
        }

        /// <summary>
        /// Gets the static content response, if possible.
        /// </summary>
        /// <param name="context">Current context</param>
        /// <returns>Response if serving content, null otherwise</returns>
        public Response GetContent(NancyContext context)
        {
            return this.conventions
                       .Select(convention => convention.Invoke(context, this.rootPathProvider.GetRootPath()))
                       .FirstOrDefault(response => response != null);
        }
    }
}