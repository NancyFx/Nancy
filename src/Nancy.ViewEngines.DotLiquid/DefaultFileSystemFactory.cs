namespace Nancy.ViewEngines.DotLiquid
{
    using Conventions;
    using global::DotLiquid.FileSystems;

    /// <summary>
    /// Default implementation of the <see cref="IFileSystemFactory"/> interface.
    /// </summary>
    /// <remarks>This implementation always returns instances of the <see cref="LiquidNancyFileSystem"/> type.</remarks>
    public class DefaultFileSystemFactory : IFileSystemFactory
    {
        private readonly ViewLocationConventions conventions;

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultFileSystemFactory"/> class
        /// </summary>
        public DefaultFileSystemFactory()
        {
        }

        /// <summary>
        /// Gets a <see cref="IFileSystem"/> instance for the provided <paramref name="context"/>.
        /// </summary>
        /// <param name="context">The context that the filesystem should be created for.</param>
        /// <returns>An <see cref="IFileSystem"/> instance.</returns>
        public IFileSystem GetFileSystem(ViewEngineStartupContext context)
        {
            return new LiquidNancyFileSystem(context);
        }
    }
}