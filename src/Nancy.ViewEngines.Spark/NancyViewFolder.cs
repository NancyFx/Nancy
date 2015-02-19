namespace Nancy.ViewEngines.Spark
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Threading;

    using global::Spark.FileSystem;

    using Nancy.Responses.Negotiation;

    /// <summary>
    /// Implementation of the IViewFolder interface to have Spark use views that's been discovered by Nancy's view locator.
    /// </summary>
    public class NancyViewFolder : IViewFolder
    {
        private readonly ViewEngineStartupContext viewEngineStartupContext;

        private List<ViewLocationResult> currentlyLocatedViews;

        private ReaderWriterLockSlim padlock = new ReaderWriterLockSlim();

        private readonly ConcurrentDictionary<string, IViewFile> cachedFiles = new ConcurrentDictionary<string, IViewFile>();

        /// <summary>
        /// Initializes a new instance of the <see cref="NancyViewFolder"/> class, using the provided
        /// <see cref="viewEngineStartupContext"/> instance.
        /// </summary>
        /// <param name="viewEngineStartupContext"></param>
        public NancyViewFolder(ViewEngineStartupContext viewEngineStartupContext)
        {
            this.viewEngineStartupContext = viewEngineStartupContext;

            // No need to lock here
            this.currentlyLocatedViews =
                new List<ViewLocationResult>(viewEngineStartupContext.ViewLocator.GetAllCurrentlyDiscoveredViews());
        }

        /// <summary>
        /// Gets the source of the requested view.
        /// </summary>
        /// <param name="path">The view to get the source for</param>
        /// <returns>A <see cref="IViewFile"/> instance.</returns>
        public IViewFile GetViewSource(string path)
        {
            var searchPath = ConvertPath(path);

            IViewFile fileResult;
            if (this.cachedFiles.TryGetValue(searchPath, out fileResult))
            {
                return fileResult;
            }

            ViewLocationResult result = null;

            this.padlock.EnterUpgradeableReadLock();
            try
            {
                result = this.currentlyLocatedViews
                             .FirstOrDefault(v => CompareViewPaths(GetSafeViewPath(v), searchPath));

                if (result == null && StaticConfiguration.Caching.EnableRuntimeViewDiscovery)
                {
                    result = this.viewEngineStartupContext.ViewLocator.LocateView(searchPath, GetFakeContext());

                    this.padlock.EnterWriteLock();
                    try
                    {
                        this.currentlyLocatedViews.Add(result);
                    }
                    finally
                    {
                        this.padlock.ExitWriteLock();
                    }
                }
            }
            finally
            {
                this.padlock.ExitUpgradeableReadLock();
            }


            if (result == null)
            {
                throw new FileNotFoundException(string.Format("Template {0} not found", path), path);
            }

            fileResult = new NancyViewFile(result);
            this.cachedFiles.AddOrUpdate(searchPath, s => fileResult, (s, o) => fileResult);

            return fileResult;
        }

        /// <summary>
        /// Lists all view for the specified <paramref name="path"/>.
        /// </summary>
        /// <param name="path">The path to return views for.</param>
        /// <returns>An <see cref="IEnumerable{T}"/> that contains the matched views.</returns>
        public IList<string> ListViews(string path)
        {
            this.padlock.EnterReadLock();

            try
            {
                return currentlyLocatedViews.
                    Where(v => v.Location.StartsWith(path, StringComparison.OrdinalIgnoreCase)).
                    Select(v =>
                        v.Location.Length == path.Length ?
                            v.Name + "." + v.Extension :
                            v.Location.Substring(path.Length) + "/" + v.Name + "." + v.Extension).
                    ToList();

            }
            finally
            {
                this.padlock.ExitReadLock();
            }
        }

        /// <summary>
        /// Gets a value that indicates whether or not the view folder contains a specific view.
        /// </summary>
        /// <param name="path">The view to check for.</param>
        /// <returns><see langword="true"/> if the view exists in the view folder; otherwise <see langword="false"/>.</returns>
        public bool HasView(string path)
        {
            var searchPath =
                ConvertPath(path);

            this.padlock.EnterUpgradeableReadLock();
            try
            {
                var hasCached = this.currentlyLocatedViews.Any(v => CompareViewPaths(GetSafeViewPath(v), searchPath));

                if (hasCached || !StaticConfiguration.Caching.EnableRuntimeViewDiscovery)
                {
                    return hasCached;
                }

                var newView = this.viewEngineStartupContext.ViewLocator.LocateView(searchPath, GetFakeContext());

                if (newView == null)
                {
                    return false;
                }

                this.padlock.EnterWriteLock();
                try
                {
                    this.currentlyLocatedViews.Add(newView);

                    return true;
                }
                finally
                {
                    this.padlock.ExitWriteLock();
                }
            }
            finally
            {
                this.padlock.ExitUpgradeableReadLock();
            }
        }

        private static bool CompareViewPaths(string storedViewPath, string requestedViewPath)
        {
            return String.Equals(storedViewPath, requestedViewPath, StringComparison.OrdinalIgnoreCase);
        }

        private static string ConvertPath(string path)
        {
            return path.Replace(@"\", "/");
        }

        private static string GetSafeViewPath(ViewLocationResult result)
        {
            return string.IsNullOrEmpty(result.Location) ? 
                string.Concat(result.Name, ".", result.Extension) : 
                string.Concat(result.Location, "/", result.Name, ".", result.Extension);
        }

        // Horrible hack, but we have no way to get a context
        private static NancyContext GetFakeContext()
        {
            return new NancyContext { Request = new Request("GET", "/", "http") };
        }

        public class NancyViewFile : IViewFile
        {
            private readonly object updateLock = new object();

            private readonly ViewLocationResult viewLocationResult;

            private string contents;

            private long lastUpdated;

            public NancyViewFile(ViewLocationResult viewLocationResult)
            {
                this.viewLocationResult = viewLocationResult;

                this.UpdateContents();
            }

            public long LastModified
            {
                get
                {
                    if (StaticConfiguration.Caching.EnableRuntimeViewUpdates && this.viewLocationResult.IsStale())
                    {
                        this.UpdateContents();
                    }

                    return this.lastUpdated;
                }
            }

            public Stream OpenViewStream()
            {
                if (StaticConfiguration.Caching.EnableRuntimeViewUpdates && this.viewLocationResult.IsStale())
                {
                    this.UpdateContents();
                }

                return new MemoryStream(Encoding.UTF8.GetBytes(this.contents));
            }

            private void UpdateContents()
            {
                lock (this.updateLock)
                {
                    using (var reader = this.viewLocationResult.Contents.Invoke())
                    {
                        this.contents = reader.ReadToEnd();
                    }

                    this.lastUpdated = DateTime.Now.Ticks;
                }
            }
        }
    }
}