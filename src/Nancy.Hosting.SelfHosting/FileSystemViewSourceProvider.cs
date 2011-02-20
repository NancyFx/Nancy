namespace Nancy.Hosting.SelfHosting
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
        /// <summary>
        /// Attemptes to locate the view, specified by the <paramref name="viewName"/> parameter, in the underlaying source.
        /// </summary>
        /// <param name="viewName">The name of the view that should be located.</param>
        /// <param name="supportedViewEngineExtensions">The supported view engine extensions that the view is allowed to use.</param>
        /// <returns>A <see cref="ViewLocationResult"/> instance if the view could be located; otherwise <see langword="null"/>.</returns>
        public ViewLocationResult LocateView(string viewName, IEnumerable<string> supportedViewEngineExtensions)
        {
            var viewFolder =
                Path.Combine(Environment.CurrentDirectory, "views");

            if (string.IsNullOrEmpty(viewFolder))
            {
                return null;
            }

            var filesInViewFolder =
                Directory.GetFiles(viewFolder);

            var viewsFiles =
                from file in filesInViewFolder
                from extension in supportedViewEngineExtensions
                where Path.GetFileName(file).Equals(string.Concat(viewName, ".", extension), StringComparison.OrdinalIgnoreCase)
                select new
                {
                    file,
                    extension
                };

            var selectedView =
                viewsFiles.FirstOrDefault();

            var fileStream = new FileStream(selectedView.file, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);

            return new ViewLocationResult(
                selectedView.file,
                selectedView.extension,
                new StreamReader(fileStream)
            );
        }
    }
}