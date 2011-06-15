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

            var viewsThatMatchesCritera = this.viewLocationCache.Where(
                x => x.Name.Equals(Path.GetFileNameWithoutExtension(viewName), StringComparison.OrdinalIgnoreCase));

            viewsThatMatchesCritera = GetViewsThatMatchesViewExtension(viewName, viewsThatMatchesCritera);

            if (viewsThatMatchesCritera.Count() > 1)
            {
                throw new AmbiguousViewsException();
            }

            return viewsThatMatchesCritera.FirstOrDefault();
        }

        private static IEnumerable<ViewLocationResult> GetViewsThatMatchesViewExtension(string viewName, IEnumerable<ViewLocationResult> viewsThatMatchesCritera)
        {
            var viewExtension = Path.GetExtension(viewName);

            if (!string.IsNullOrEmpty(viewExtension))
            {
                viewsThatMatchesCritera = viewsThatMatchesCritera.Where(x => x.Extension.Equals(viewExtension.Substring(1), StringComparison.OrdinalIgnoreCase));    
            }

            return viewsThatMatchesCritera;
        }
    }
}