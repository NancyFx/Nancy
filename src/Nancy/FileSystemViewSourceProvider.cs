namespace Nancy
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using Nancy.ViewEngines;

    /// <summary>
    /// Contains the functionality for locating a view that is located on the file system.
    /// </summary>
    public class FileSystemViewSourceProvider : IViewSourceProvider
    {
        private readonly IRootPathProvider rootPathProvider;

        public FileSystemViewSourceProvider(IRootPathProvider rootPathProvider)
        {
            this.rootPathProvider = rootPathProvider;
        }

        /// <summary>
        /// Attemptes to locate the view, specified by the <paramref name="viewName"/> parameter, in the underlaying source.
        /// </summary>
        /// <param name="viewName">The name of the view that should be located.</param>
        /// <param name="supportedViewEngineExtensions">The supported view engine extensions that the view is allowed to use.</param>
        /// <returns>A <see cref="ViewLocationResult"/> instance if the view could be located; otherwise <see langword="null"/>.</returns>
        public ViewLocationResult LocateView(string viewName, IEnumerable<string> supportedViewEngineExtensions)
        {
            var viewFolder =
                Path.Combine(this.rootPathProvider.GetRootPath(), "views");

            if (string.IsNullOrEmpty(viewFolder))
            {
                return null;
            }

            var selectedView = FindViewFromName(viewFolder, viewName, supportedViewEngineExtensions);
            var fileStream = new FileStream(selectedView.Item1, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);

            return new ViewLocationResult(
                selectedView.Item1,
                selectedView.Item2,
                new StreamReader(fileStream)
            );
        }

        private static Tuple<string, string> FindViewFromName(string viewFolder, string viewName, IEnumerable<string> supportedViewEngineExtensions)
        {
            foreach (var extension in supportedViewEngineExtensions)
            {
                var file = Path.Combine(viewFolder, string.Concat(viewName, ".", extension));

                if (File.Exists(file))
                {
                    return Tuple.Create(file, extension);
                }
            }

            return null;
        }
    }
}