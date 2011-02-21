namespace Nancy.ViewEngines
{
    using System.Collections.Generic;

    /// <summary>
    /// Defines the functionality for locating a view at a specific source.
    /// </summary>
    public interface IViewSourceProvider
    {
        /// <summary>
        /// Attemptes to locate the view, specified by the <paramref name="viewName"/> parameter, in the underlaying source.
        /// </summary>
        /// <param name="viewName">The name of the view that should be located.</param>
        /// <param name="supportedViewEngineExtensions">The supported view engine extensions that the view is allowed to use.</param>
        /// <returns>A <see cref="ViewLocationResult"/> instance if the view could be located; otherwise <see langword="null"/>.</returns>
        ViewLocationResult LocateView(string viewName, IEnumerable<string> supportedViewEngineExtensions);
    }
}