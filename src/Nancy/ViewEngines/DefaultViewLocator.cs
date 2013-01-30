namespace Nancy.ViewEngines
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Threading;

    /// <summary>
    /// The default implementation for how views are located by Nancy.
    /// </summary>
    public class DefaultViewLocator : IViewLocator
    {
        private readonly List<ViewLocationResult> viewLocationResults;

        private readonly IViewLocationProvider viewLocationProvider;

        private readonly IEnumerable<IViewEngine> viewEngines;

        private readonly ReaderWriterLockSlim padlock;

        public DefaultViewLocator(IViewLocationProvider viewLocationProvider, IEnumerable<IViewEngine> viewEngines)
        {
            this.viewLocationProvider = viewLocationProvider;
            this.viewEngines = viewEngines;

            // No need to lock here, we get constructed on app startup
            this.viewLocationResults = new List<ViewLocationResult>(this.GetInititialViewLocations());
        }

        /// <summary>
        /// Gets the location of the view defined by the <paramref name="viewName"/> parameter.
        /// </summary>
        /// <param name="viewName">Name of the view to locate.</param>
        /// <param name="context">The <see cref="NancyContext"/> instance for the current request.</param>
        /// <returns>A <see cref="ViewLocationResult"/> instance if the view could be located; otherwise <see langword="null"/>.</returns>
        public ViewLocationResult LocateView(string viewName, NancyContext context)
        {
            if (string.IsNullOrEmpty(viewName))
            {
                return null;
            }

            this.padlock.EnterUpgradeableReadLock();
            try
            {
                var cachedResults = this.GetCachedMatchingViews(viewName);
                if (cachedResults.Length == 1)
                {
                    return cachedResults.First();
                }

                if (cachedResults.Length > 1)
                {
                    throw new AmbiguousViewsException(GetAmgiguousViewExceptionMessage(cachedResults.Length, cachedResults));
                }

                var uncachedResults = this.GetUncachedMatchingViews(viewName);
                if (!uncachedResults.Any())
                {
                    return null;
                }

                this.padlock.EnterWriteLock();
                try
                {
                    this.viewLocationResults.AddRange(uncachedResults);
                }
                finally
                {
                    this.padlock.ExitWriteLock();
                }

                if (uncachedResults.Length > 1)
                {
                    throw new AmbiguousViewsException(GetAmgiguousViewExceptionMessage(uncachedResults.Length, uncachedResults));
                }

                return uncachedResults.First();
            }
            finally
            {
                this.padlock.ExitUpgradeableReadLock();                    
            }
        }

        private ViewLocationResult[] GetUncachedMatchingViews(string viewName)
        {
            var supportedViewExtensions =
                GetSupportedViewExtensions();

            return this.viewLocationProvider.GetLocatedViews(supportedViewExtensions, viewName)
                                            .ToArray();
        }

        private ViewLocationResult[] GetCachedMatchingViews(string viewName)
        {
            return this.viewLocationResults.Where(x => NameMatchesView(viewName, x))
                       .Where(x => ExtensionMatchesView(viewName, x))
                       .Where(x => LocationMatchesView(viewName, x))
                       .ToArray();
        }

        private IEnumerable<ViewLocationResult> GetInititialViewLocations()
        {
            var supportedViewExtensions =
                GetSupportedViewExtensions();

            var viewsLocatedByProviders =
                this.viewLocationProvider.GetLocatedViews(supportedViewExtensions);

            return viewsLocatedByProviders.ToArray();
        }

        private IEnumerable<string> GetSupportedViewExtensions()
        {
            return this.viewEngines
                .SelectMany(engine => engine.Extensions)
                .Distinct();
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
            var filename = Path.GetFileName(viewName);
            var index = viewName.LastIndexOf(filename, System.StringComparison.OrdinalIgnoreCase);
            var location = index >= 0 ? viewName.Remove(index, filename.Length) : viewName;
            location = location.TrimEnd(new[] { '/' });

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