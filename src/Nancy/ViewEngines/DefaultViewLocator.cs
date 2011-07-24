namespace Nancy.ViewEngines
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;

    /// <summary>
    /// The default implementation for how views are located by Nancy.
    /// </summary>
    public class DefaultViewLocator : IViewLocator
    {
        private readonly IViewLocationCache viewLocationCache;

        public DefaultViewLocator(IViewLocationCache viewLocationCache)
        {
            this.viewLocationCache = viewLocationCache;
        }

        /// <summary>
        /// Gets the location of the view defined by the <paramref name="viewName"/> parameter.
        /// </summary>
        /// <param name="viewName">Name of the view to locate.</param>
        /// <returns>A <see cref="ViewLocationResult"/> instance if the view could be located; otherwise <see langword="null"/>.</returns>
        public ViewLocationResult LocateView(string viewName)
        {
            if (string.IsNullOrEmpty(viewName))
            {
                return null;
            }

            var viewsThatMatchesCritera = this.viewLocationCache
                .Where(x => NameMatchesView(viewName, x))
                .Where(x => ExtensionMatchesView(viewName, x))
                .Where(x => LocationMatchesView(viewName, x))
                .ToList();

            var count = viewsThatMatchesCritera.Count();
            if (count > 1)
            {
                throw new AmbiguousViewsException(GetAmgiguousViewExceptionMessage(count, viewsThatMatchesCritera));
            }

            return viewsThatMatchesCritera.SingleOrDefault();
        }

        private static string GetAmgiguousViewExceptionMessage(int count, IEnumerable<ViewLocationResult> viewsThatMatchesCritera)
        {
            return string.Format("This exception was thrown because multiple views were found. {0} view(s):\r\n\t{1}", count, string.Join("\r\n\t", viewsThatMatchesCritera.Select(GetFullLocationOfView).ToArray()));
        }

        private static string GetFullLocationOfView(ViewLocationResult viewLocationResult)
        {
            return string.Concat(viewLocationResult.Location, "/", viewLocationResult.Name, ".", viewLocationResult.Extension);
        }

        private static bool ExtensionMatchesView(string viewName, ViewLocationResult viewLocationResult)
        {
            var extension = Path.GetExtension(viewName);

            return string.IsNullOrEmpty(extension) ||
                viewLocationResult.Extension.Equals(extension.Substring(1), StringComparison.OrdinalIgnoreCase);
        }

        private static bool LocationMatchesView(string viewName, ViewLocationResult viewLocationResult)
        {
            var location = viewName
                .Replace(Path.GetFileName(viewName), string.Empty)
                .TrimEnd(new [] { '/' });

            return viewLocationResult.Location.Equals(location, StringComparison.OrdinalIgnoreCase);
        }

        private static bool NameMatchesView(string viewName, ViewLocationResult viewLocationResult)
        {
            var name = Path.GetFileNameWithoutExtension(viewName);

            return (!string.IsNullOrEmpty(name)) &&
                viewLocationResult.Name.Equals(name, StringComparison.OrdinalIgnoreCase);
        }
    }
}