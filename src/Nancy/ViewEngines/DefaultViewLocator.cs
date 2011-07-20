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

            var viewThatMatchCriteria = this.viewLocationCache
                .Where(x => NameMatchesView(viewName, x))
                .Where(x => ExtensionMatchesView(viewName, x))
                .Where(x => LocationMatchesView(viewName, x))
                .ToList();

            if (viewThatMatchCriteria.Count() > 1)
            {
                throw new AmbiguousViewsException();
            }

            return viewThatMatchCriteria.SingleOrDefault();
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