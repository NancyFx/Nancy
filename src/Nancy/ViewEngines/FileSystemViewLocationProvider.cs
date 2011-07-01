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
        private readonly IRootPathProvider rootPathProvider;

        /// <summary>
        /// Initializes a new instance of the <see cref="FileSystemViewLocationProvider"/> class.
        /// </summary>
        /// <param name="rootPathProvider">A <see cref="IRootPathProvider"/> instance..</param>
        public FileSystemViewLocationProvider(IRootPathProvider rootPathProvider)
        {
            this.rootPathProvider = rootPathProvider;
        }

        /// <summary>
        /// Returns an <see cref="ViewLocationResult"/> instance for all the views that could be located by the provider.
        /// </summary>
        /// <param name="supportedViewExtensions">An <see cref="IEnumerable{T}"/> instance, containing the view engine file extensions that is supported by the running instance of Nancy.</param>
        /// <returns>An <see cref="IEnumerable{T}"/> instance, containing <see cref="ViewLocationResult"/> instances for the located views.</returns>
        /// <remarks>If no views could be located, this method should return an empty enumerable, never <see langword="null"/>.</remarks>
        public IEnumerable<ViewLocationResult> GetLocatedViews(IEnumerable<string> supportedViewExtensions)
        {
            return
                from match in GetViewsWithSupportedExtensions(supportedViewExtensions)
                select new ViewLocationResult(
                    match.Replace(this.rootPathProvider.GetRootPath(), string.Empty).TrimStart(new[] { Path.DirectorySeparatorChar }),
                    Path.GetFileNameWithoutExtension(match),
                    Path.GetExtension(match).Substring(1),
                    () => new StreamReader(new FileStream(match, FileMode.Open, FileAccess.Read, FileShare.ReadWrite)));
        }

        private IEnumerable<string> GetViewsWithSupportedExtensions(IEnumerable<string> supportedViewExtensions)
        {
            return supportedViewExtensions
                .SelectMany(extensions => Directory.GetFiles(this.rootPathProvider.GetRootPath(),
                string.Concat("*.", extensions),
                SearchOption.AllDirectories)
                ).Distinct().ToList();
        }
    }
}