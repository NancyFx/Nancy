namespace Nancy.ViewEngines.Spark
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;
    using global::Spark.FileSystem;

    /// <summary>
    /// Implementation of the IViewFolder interface to have Spark use views that's been discovered by Nancy's view locator.
    /// </summary>
    public class NancyViewFolder : IViewFolder
    {
        private readonly ViewEngineStartupContext viewEngineStartupContext;

        /// <summary>
        /// Initializes a new instance of the <see cref="NancyViewFolder"/> class, using the provided
        /// <see cref="viewEngineStartupContext"/> instance.
        /// </summary>
        /// <param name="viewEngineStartupContext"></param>
        public NancyViewFolder(ViewEngineStartupContext viewEngineStartupContext)
        {
            this.viewEngineStartupContext = viewEngineStartupContext;
        }

        /// <summary>
        /// Gets the source of the requested view.
        /// </summary>
        /// <param name="path">The view to get the source for</param>
        /// <returns>A <see cref="IViewFile"/> instance.</returns>
        public IViewFile GetViewSource(string path)
        {
            throw new NotImplementedException();
            //var searchPath = ConvertPath(path);

            //var viewLocationResult = this.viewEngineStartupContext.ViewLocationResults
            //    .FirstOrDefault(v => CompareViewPaths(GetSafeViewPath(v), searchPath));

            //if (viewLocationResult == null)
            //{
            //    throw new FileNotFoundException(string.Format("Template {0} not found", path), path);
            //}

            //return new NancyViewFile(viewLocationResult);
        }

        /// <summary>
        /// Lists all view for the specified <paramref name="path"/>.
        /// </summary>
        /// <param name="path">The path to return views for.</param>
        /// <returns>An <see cref="IEnumerable{T}"/> that contains the matched views.</returns>
        public IList<string> ListViews(string path)
        {
            throw new NotImplementedException();
            //return this.viewEngineStartupContext.
            //    ViewLocationResults.
            //    Where(v => v.Location.StartsWith(path, StringComparison.OrdinalIgnoreCase)).
            //    Select(v =>
            //        v.Location.Length == path.Length ?
            //            v.Name + "." + v.Extension : 
            //            v.Location.Substring(path.Length) + "/" + v.Name + "." + v.Extension).
            //    ToList();
        }

        /// <summary>
        /// Gets a value that indicates wether or not the view folder contains a specific view.
        /// </summary>
        /// <param name="path">The view to check for.</param>
        /// <returns><see langword="true"/> if the view exists in the view folder; otherwise <see langword="false"/>.</returns>
        public bool HasView(string path)
        {
            throw new NotImplementedException();
            //var searchPath = 
            //    ConvertPath(path);

            //return this.viewEngineStartupContext.ViewLocationResults.Any(v => CompareViewPaths(GetSafeViewPath(v), searchPath));
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

        public class NancyViewFile : IViewFile
        {
            private readonly ViewLocationResult viewLocationResult;
            private readonly long created;

            public NancyViewFile(ViewLocationResult viewLocationResult)
            {
                this.viewLocationResult = viewLocationResult;
                this.created = DateTime.Now.Ticks;
            }

            public long LastModified
            {
                get { return StaticConfiguration.DisableCaches ? DateTime.Now.Ticks : this.created; }
            }

            public Stream OpenViewStream()
            {
                string view;
                using (var reader = this.viewLocationResult.Contents.Invoke())
                {
                    view = reader.ReadToEnd();
                }

                return new MemoryStream(Encoding.UTF8.GetBytes(view));
            }
        }
    }
}