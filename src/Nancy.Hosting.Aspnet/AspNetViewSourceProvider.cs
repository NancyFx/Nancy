namespace Nancy.Hosting.Aspnet
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Web.Hosting;
    using ViewEngines;

    /// <summary>
    /// Contains the functionality for locating a view that is hosted on ASP.NET.
    /// </summary>
    public class AspNetViewSourceProvider : IViewSourceProvider
    {
        /// <summary>
        /// Attemptes to locate the view, specified by the <paramref name="viewName"/> parameter, in the underlaying source.
        /// </summary>
        /// <param name="viewName">The name of the view that should be located.</param>
        /// <param name="supportedViewEngineExtensions">The supported view engine extensions that the view is allowed to use.</param>
        /// <returns>A <see cref="ViewLocationResult"/> instance if the view could be located; otherwise <see langword="null"/>.</returns>
        /// <remarks>This source provider attempts to locate the view in the <c>~/views</c> folder.</remarks>
        public ViewLocationResult LocateView(string viewName, IEnumerable<string> supportedViewEngineExtensions)
        {
            var viewFolder =
                HostingEnvironment.MapPath("~/views");

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
                select new {
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