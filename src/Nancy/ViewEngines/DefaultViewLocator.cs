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
        private readonly IEnumerable<IViewLocationProvider> viewLocationProviders;
        private readonly IEnumerable<IViewEngine> viewEngines;
        private readonly IEnumerable<ViewLocationResult> locatedViews;

        public DefaultViewLocator(IEnumerable<IViewLocationProvider> viewLocationProviders, IEnumerable<IViewEngine> viewEngines)
        {
            this.viewLocationProviders = viewLocationProviders;
            this.viewEngines = viewEngines;
            this.locatedViews = GetLocatedViews();
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

            var viewsThatMatchesCritera = this.locatedViews
                .Where(x => x.Name.Equals(Path.GetFileNameWithoutExtension(viewName), StringComparison.OrdinalIgnoreCase));

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

        private IEnumerable<ViewLocationResult> GetLocatedViews()
        {
            var supportedViewExtensions = 
                GetSupportedViewExtensions();

            var viewsLocatedByProviders = this.viewLocationProviders
                .SelectMany(x => x.GetLocatedViews(supportedViewExtensions))
                .ToList();

            return viewsLocatedByProviders;
        }

        private IEnumerable<string> GetSupportedViewExtensions()
        {
            return this.viewEngines
                .SelectMany(engine => engine.Extensions)
                .Distinct();
        }
    }
}