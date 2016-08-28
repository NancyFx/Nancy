namespace Nancy.ViewEngines
{
    using System;
    using System.Linq;

    /// <summary>
    /// Exception that is thrown when a view could not be located.
    /// </summary>
    public class ViewNotFoundException : Exception
    {
        private readonly IRootPathProvider rootPathProvider;

        /// <summary>
        /// Gets the name of the view.
        /// </summary>
        /// <value>
        /// The name of the view.
        /// </value>
        public string ViewName { get; private set; }

        /// <summary>
        /// Gets the available view engine extensions.
        /// </summary>
        /// <value>
        /// The available view engine extensions.
        /// </value>
        public string[] AvailableViewEngineExtensions { get; private set; }

        /// <summary>
        /// Gets the inspected locations.
        /// </summary>
        /// <value>
        /// The inspected locations.
        /// </value>
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

            this.message = string.Format(
                "{4}Unable to locate requested view{4}{4} \u2022 Name: {0}{4} \u2022 Root path: {3}{4} \u2022 Supported extensions: {4}{1} \u2022 Inspected locations: {4}{2}{4}" +
                "If you were expecting raw data back, make sure you set the 'Accept'-header of the request to correct format, for example 'application/json'{4}",
                this.ViewName,
                string.Join(string.Empty, this.AvailableViewEngineExtensions.Select(x => string.Concat("  - ", x, Environment.NewLine))),
                string.Join(string.Empty, this.InspectedLocations.Select(x => string.Concat("  - ", x, Environment.NewLine))),
                this.rootPathProvider.GetRootPath(),
                Environment.NewLine);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ViewNotFoundException"/>.
        /// </summary>
        /// <param name="viewName">The name of the view that was being located.</param>
        /// <param name="availableViewEngineExtensions">List of available view extensions that can be rendered by the available view engines.</param>
        public ViewNotFoundException(string viewName, string[] availableViewEngineExtensions)
        {
            this.ViewName = viewName;
            this.AvailableViewEngineExtensions = availableViewEngineExtensions;

            this.message = String.Format(
                    "Unable to locate view '{0}'{2}Currently available view engine extensions: {1}{2}",
                    this.ViewName,
                    string.Join(",", this.AvailableViewEngineExtensions),
                    Environment.NewLine);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ViewNotFoundException"/>
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