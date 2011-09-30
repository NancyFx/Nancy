namespace Nancy.ViewEngines
{
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;

    /// <summary>
    /// Contains the functionality for locating a view that is located on the file system.
    /// </summary>
    public class FileSystemViewLocationProvider : IViewLocationProvider
    {
        private readonly IFileSystemReader fileSystemReader;
        private readonly string rootPath;

        /// <summary>
        /// Initializes a new instance of the <see cref="FileSystemViewLocationProvider"/> class.
        /// </summary>
        /// <param name="rootPathProvider">A <see cref="IRootPathProvider"/> instance.</param>
        /// <remarks>Creating an instance using this constructor will result in the <see cref="DefaultFileSystemReader"/> being used internally.</remarks>
        public FileSystemViewLocationProvider(IRootPathProvider rootPathProvider)
            : this(rootPathProvider, new DefaultFileSystemReader())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FileSystemViewLocationProvider"/> class.
        /// </summary>
        /// <param name="rootPathProvider">A <see cref="IRootPathProvider"/> instance.</param>
        /// <param name="fileSystemReader">An <see cref="IFileSystemReader"/> instance that should be used when retrieving view information from the file system.</param>
        public FileSystemViewLocationProvider(IRootPathProvider rootPathProvider, IFileSystemReader fileSystemReader)
        {
            this.fileSystemReader = fileSystemReader;
            this.rootPath = rootPathProvider.GetRootPath();
        }

        /// <summary>
        /// Returns an <see cref="ViewLocationResult"/> instance for all the views that could be located by the provider.
        /// </summary>
        /// <param name="supportedViewExtensions">An <see cref="IEnumerable{T}"/> instance, containing the view engine file extensions that is supported by the running instance of Nancy.</param>
        /// <returns>An <see cref="IEnumerable{T}"/> instance, containing <see cref="ViewLocationResult"/> instances for the located views.</returns>
        /// <remarks>If no views could be located, this method should return an empty enumerable, never <see langword="null"/>.</remarks>
        public IEnumerable<ViewLocationResult> GetLocatedViews(IEnumerable<string> supportedViewExtensions)
        {
            if (string.IsNullOrEmpty(this.rootPath))
            {
                return Enumerable.Empty<ViewLocationResult>();
            }

            var matches =
                this.fileSystemReader.GetViewsWithSupportedExtensions(this.rootPath, supportedViewExtensions);

            return
                from match in matches
                select new ViewLocationResult(
                    GetViewLocation(match.Item1, rootPath),
                    Path.GetFileNameWithoutExtension(match.Item1),
                    Path.GetExtension(match.Item1).Substring(1),
                    match.Item2);
        }

        private static string GetViewLocation(string match, string rootPath)
        {
            var location = match
                .Replace(rootPath, string.Empty)
                .TrimStart(new[] { Path.DirectorySeparatorChar })
                .Replace(@"\", "/")
                .Replace(Path.GetFileName(match), string.Empty)
                .TrimEnd(new [] { '/' });

            return location;
        }
    }
}