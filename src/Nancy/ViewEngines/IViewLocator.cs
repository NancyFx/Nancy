namespace Nancy.ViewEngines
{
    using System.Collections.Generic;

    /// <summary>
    /// Defines the functionality for locating the requested view.
    /// </summary>
    public interface IViewLocator
    {
        /// <summary>
        /// Gets the location of the view defined by the <paramref name="viewName"/> parameter.
        /// </summary>
        /// <param name="viewName">Name of the view to locate.</param>
        /// <param name="supportedViewEngineExtensions">An <see cref="IEnumerable{T}"/> instance containing the supported view engine extensions.</param>
        /// <returns>A <see cref="ViewLocationResult"/> instance if the view could be located; otherwise <see langword="null"/>.</returns>
        ViewLocationResult GetViewLocation(string viewName, IEnumerable<string> supportedViewEngineExtensions);
    }
}