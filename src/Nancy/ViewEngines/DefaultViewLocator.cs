namespace Nancy.ViewEngines
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Threading;
    using Configuration;

    /// <summary>
    /// The default implementation of <see cref="IViewLocator"/>.
    /// </summary>
    public class DefaultViewLocator : IViewLocator
    {
        private readonly List<ViewLocationResult> viewLocationResults;
        private readonly IViewLocationProvider viewLocationProvider;
        private readonly IEnumerable<IViewEngine> viewEngines;
        private readonly ReaderWriterLockSlim padlock = new ReaderWriterLockSlim();
        private readonly char[] invalidCharacters;
        private readonly ViewConfiguration configuration;

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultViewLocator"/> class.
        /// </summary>
        /// <param name="viewLocationProvider">An <see cref="IViewLocationProvider"/> instance.</param>
        /// <param name="viewEngines">An <see cref="IEnumerable{T}"/> of <see cref="IViewEngine"/> instances.</param>
        /// <param name="environment">An <see cref="INancyEnvironment"/> instance.</param>
        public DefaultViewLocator(IViewLocationProvider viewLocationProvider, IEnumerable<IViewEngine> viewEngines, INancyEnvironment environment)
        {
            this.viewLocationProvider = viewLocationProvider;
            this.viewEngines = viewEngines;
            this.invalidCharacters = Path.GetInvalidFileNameChars().Where(c => c != '/').ToArray();
            this.viewLocationResults = new List<ViewLocationResult>(this.GetInititialViewLocations());
            this.configuration = environment.GetValue<ViewConfiguration>();
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

            if (!this.IsValidViewName(viewName))
            {
                return null;
            }

            // If we can't do runtime discovery there's no need to lock anything
            // as we can assume our cache is immutable.
            if (!this.configuration.RuntimeViewDiscovery)
            {
                return this.LocateCachedView(viewName);
            }

            this.padlock.EnterUpgradeableReadLock();
            try
            {
                var cachedResult = this.LocateCachedView(viewName);

                if (cachedResult != null)
                {
                    return cachedResult;
                }

                return !this.configuration.RuntimeViewDiscovery
                    ? null
                    : this.LocateAndCacheUncachedView(viewName);
            }
            finally
            {
                this.padlock.ExitUpgradeableReadLock();
            }
        }

        /// <summary>
        /// Gets all the views that are currently discovered
        /// Note: this is *not* the recommended way to deal with the view locator
        /// as it doesn't allow for runtime discovery of views with the
        /// <see cref="ViewConfiguration"/>.
        /// </summary>
        /// <returns>A collection of <see cref="ViewLocationResult"/> instances</returns>
        public IEnumerable<ViewLocationResult> GetAllCurrentlyDiscoveredViews()
        {
            this.padlock.EnterReadLock();

            try
            {
                // Make a copy to avoid any modification issues
                var newList = new List<ViewLocationResult>(this.viewLocationResults.Count);
                this.viewLocationResults.ForEach(newList.Add);
                return newList;
            }
            finally
            {
                this.padlock.ExitReadLock();
            }
        }

        private ViewLocationResult LocateAndCacheUncachedView(string viewName)
        {
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

        private ViewLocationResult LocateCachedView(string viewName)
        {
            var cachedResults = this.GetCachedMatchingViews(viewName);
            if (cachedResults.Length == 1)
            {
                return cachedResults.Single();
            }

            if (cachedResults.Length > 1)
            {
                throw new AmbiguousViewsException(GetAmgiguousViewExceptionMessage(cachedResults.Length, cachedResults));
            }

            return null;
        }

        private ViewLocationResult[] GetUncachedMatchingViews(string viewName)
        {
            var viewExtension = GetExtensionFromViewName(viewName);

            var supportedViewExtensions = string.IsNullOrEmpty(viewExtension)
                ? this.GetSupportedViewExtensions()
                : new[] { viewExtension };

            var location = GetLocationFromViewName(viewName);
            var nameWithoutExtension = GetFilenameWithoutExtensionFromViewName(viewName);

            return this.viewLocationProvider.GetLocatedViews(supportedViewExtensions, location, nameWithoutExtension)
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
                this.GetSupportedViewExtensions();

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
            var extension = GetExtensionFromViewName(viewName);

            return string.IsNullOrEmpty(extension) ||
                viewLocationResult.Extension.Equals(extension, StringComparison.OrdinalIgnoreCase);
        }

        private static bool LocationMatchesView(string viewName, ViewLocationResult viewLocationResult)
        {
            var location = GetLocationFromViewName(viewName);

            return viewLocationResult.Location.Equals(location, StringComparison.OrdinalIgnoreCase);
        }

        private static bool NameMatchesView(string viewName, ViewLocationResult viewLocationResult)
        {
            var name = GetFilenameWithoutExtensionFromViewName(viewName);

            return (!string.IsNullOrEmpty(name)) &&
                viewLocationResult.Name.Equals(name, StringComparison.OrdinalIgnoreCase);
        }

        private static string GetFilenameWithoutExtensionFromViewName(string viewName)
        {
            return Path.GetFileNameWithoutExtension(viewName);
        }

        private static string GetLocationFromViewName(string viewName)
        {
            var filename = Path.GetFileName(viewName);
            var index = viewName.LastIndexOf(filename, StringComparison.OrdinalIgnoreCase);
            var location = index >= 0 ? viewName.Remove(index, filename.Length) : viewName;
            location = location.TrimEnd('/');
            return location;
        }

        private static string GetExtensionFromViewName(string viewName)
        {
            var extension = Path.GetExtension(viewName);

            return !string.IsNullOrEmpty(extension) ? extension.Substring(1) : extension;
        }

        private bool IsValidViewName(string viewName)
        {
            return !this.invalidCharacters.Any(viewName.Contains);
        }
    }
}
