namespace Nancy.ViewEngines.DotLiquid
{
    using global::DotLiquid.FileSystems;

    /// <summary>
    /// Factory for creating a <see cref="IFileSystem"/> instance.
    /// </summary>
    public interface IFileSystemFactory
    {
        /// <summary>
        /// Gets a <see cref="IFileSystem"/> instance for the provided <paramref name="context"/>.
        /// </summary>
        /// <param name="context">The context that the filesystem should be created for.</param>
        /// <returns>
        /// An <see cref="IFileSystem"/> instance.
        /// </returns>
        IFileSystem GetFileSystem(ViewEngineStartupContext context);
    }
}