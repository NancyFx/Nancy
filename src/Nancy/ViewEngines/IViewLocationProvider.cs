namespace Nancy.ViewEngines
{
    using System.Collections.Generic;

    /// <summary>
    /// Defines the functionality used by Nancy to located a view.
    /// </summary>
    public interface IViewLocationProvider
    {
        /// <summary>
        /// Returns an <see cref="ViewLocationResult"/> instance for all the views that could be located by the provider.
        /// </summary>
        /// <param name="supportedViewExtensions">An <see cref="IEnumerable{T}"/> instance, containing the view engine file extensions that is supported by the running instance of Nancy.</param>
        /// <returns>An <see cref="IEnumerable{T}"/> instance, containing <see cref="ViewLocationResult"/> instances for the located views.</returns>
        /// <remarks>If no views could be located, this method should return an empty enumerable, never <see langword="null"/>.</remarks>
        IEnumerable<ViewLocationResult> GetLocatedViews(IEnumerable<string> supportedViewExtensions);

        /// <summary>
        /// Returns an <see cref="ViewLocationResult"/> instance for all the views matching the viewName that could be located by the provider.
        /// </summary>
        /// <param name="supportedViewExtensions">An <see cref="IEnumerable{T}"/> instance, containing the view engine file extensions that is supported by the running instance of Nancy.</param>
        /// <param name="location">Location of the view</param>
        /// <param name="viewName">The name of the view to try and find</param>
        /// <returns>An <see cref="IEnumerable{T}"/> instance, containing <see cref="ViewLocationResult"/> instances for the located views.</returns>
        /// <remarks>If no views could be located, this method should return an empty enumerable, never <see langword="null"/>.</remarks>
        IEnumerable<ViewLocationResult> GetLocatedViews(IEnumerable<string> supportedViewExtensions, string location, string viewName);
    }
}