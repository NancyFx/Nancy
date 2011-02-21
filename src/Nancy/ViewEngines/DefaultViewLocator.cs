namespace Nancy.ViewEngines
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// The default implementation for how views are located by Nancy.
    /// </summary>
    public class DefaultViewLocator : IViewLocator
    {
        private readonly IEnumerable<IViewSourceProvider> viewSourceProviders;

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultViewLocator"/> class.
        /// </summary>
        /// <param name="viewSourceProviders">An <see cref="IEnumerable{T}"/> instance, containing the <see cref="IViewSourceProvider"/> used by the locator to look for a view.</param>
        public DefaultViewLocator(IEnumerable<IViewSourceProvider> viewSourceProviders)
        {
            if (viewSourceProviders == null)
            {
                throw new ArgumentNullException("viewSourceProviders", "The value of the viewSourceProviders parameter cannot be null.");
            }

            this.viewSourceProviders = viewSourceProviders;
        }

        /// <summary>
        /// Gets the location of the view defined by the <paramref name="viewName"/> parameter.
        /// </summary>
        /// <param name="viewName">Name of the view to locate.</param>
        /// <param name="supportedViewEngineExtensions">An <see cref="IEnumerable{T}"/> instance containing the supported view engine extensions.</param>
        /// <returns>A <see cref="ViewLocationResult"/> instance if the requested view could be located; otherwise <see langword="null"/>.</returns>
        public ViewLocationResult GetViewLocation(string viewName, IEnumerable<string> supportedViewEngineExtensions)
        {
            if(this.NotEnoughInformationAvailableToLocateView(viewName, supportedViewEngineExtensions))
            {
                return null;
            }

            return this.LocateView(viewName, supportedViewEngineExtensions);
        }

        private bool NotEnoughInformationAvailableToLocateView(string viewName, IEnumerable<string> supportedViewEngineExtensions)
        {
            if (viewName == null)
            {
                return true;
            }

            if (viewName.Length == 0)
            {
                return true;
            }

            if (supportedViewEngineExtensions == null)
            {
                return true;
            }

            if (!supportedViewEngineExtensions.Any())
            {
                return true;
            }

            return !this.viewSourceProviders.Any();
        }

        private ViewLocationResult LocateView(string viewName, IEnumerable<string> supportedViewEngineExtensions)
        {
            foreach (var viewSourceProvider in viewSourceProviders)
            {
                var result =
                    LocateViewAndSupressExceptions(viewSourceProvider, viewName, supportedViewEngineExtensions);

                if (result != null)
                {
                    return result;
                }
            }

            return null;
        }

        private static ViewLocationResult LocateViewAndSupressExceptions(IViewSourceProvider viewSourceProvider, string viewName, IEnumerable<string> supportedViewEngineExtensions)
        {
            try
            {
                return viewSourceProvider.LocateView(viewName, supportedViewEngineExtensions);
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}