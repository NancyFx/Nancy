namespace Nancy.ViewEngines
{
    using System;

    /// <summary>
    /// Exception that is thrown when a view could not be located.
    /// </summary>
    public class ViewNotFoundException : Exception
    {
        private readonly IRootPathProvider rootPathProvider;
        public string ViewName { get; private set; }

        public string[] AvailableViewEngineExtensions { get; private set; }
        public string[] InspectedLocations { get; private set; }

        private string message;

        /// <summary>
        /// Initializes a new instance of the <see cref="ViewNotFoundException"/>.
        /// </summary>
        /// <param name="viewName">The name of the view that was being located.</param>
        /// <param name="availableViewEngineExtensions">List of available view extensions that can be rendered by the available view engines.</param>
        /// <param name="inspectedLocations">The locations that were inspected for the view.</param>
        /// <param name="rootPathProvider">An <see cref="IRootPathProvider"/> instance.</param>
        public ViewNotFoundException(string viewName, string[] availableViewEngineExtensions, string[] inspectedLocations, IRootPathProvider rootPathProvider)
        {
            this.rootPathProvider = rootPathProvider;
            this.ViewName = viewName;
            this.AvailableViewEngineExtensions = availableViewEngineExtensions;
            this.InspectedLocations = inspectedLocations;

            this.message = String.Format(
                    "Unable to locate view '{0}'{4}Currently available view engine extensions: {1}{4}Locations inspected: {2}{4}Root path: {3}{4}" +
                    "If you were expecting raw data back, make sure you set the 'Accept'-header of the request to correct format, for example 'application/json'",
                    this.ViewName,
                    string.Join(",", this.AvailableViewEngineExtensions),
                    string.Join(",", this.InspectedLocations),
                    this.rootPathProvider.GetRootPath(),
                    Environment.NewLine);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ViewNotFoundExcepton"/>
        /// </summary>
        /// <param name="msg">A message describing the problem</param>
        public ViewNotFoundException(string msg)
        {
            this.message = msg;
        }

        /// <summary>
        /// Gets a message that describes the current exception.
        /// </summary>
        /// <returns>The error message that explains the reason for the exception, or an empty string("").</returns>
        public override string Message
        {
            get { return message; }
        }
    }
}