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
        private readonly string rootPath;

        /// <summary>
        /// Initializes a new instance of the <see cref="FileSystemViewLocationProvider"/> class.
        /// </summary>
        /// <param name="rootPathProvider">A <see cref="IRootPathProvider"/> instance..</param>
        public FileSystemViewLocationProvider(IRootPathProvider rootPathProvider)
        {
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

            return
                from match in GetViewsWithSupportedExtensions(supportedViewExtensions)
                select new ViewLocationResult(
                    GetViewLocation(match, rootPath),
                    Path.GetFileNameWithoutExtension(match),
                    Path.GetExtension(match).Substring(1),
                    () => new StreamReader(new FileStream(match, FileMode.Open, FileAccess.Read, FileShare.ReadWrite)));
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

        private IEnumerable<string> GetViewsWithSupportedExtensions(IEnumerable<string> supportedViewExtensions)
        {
            return supportedViewExtensions
                .SelectMany(extensions => Directory.GetFiles(this.rootPath,
                string.Concat("*.", extensions),
                SearchOption.AllDirectories)
                ).Distinct().ToList();
        }
    }
}