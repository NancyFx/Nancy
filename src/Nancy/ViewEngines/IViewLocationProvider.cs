namespace Nancy.ViewEngines
{
    using System.Collections.Generic;

    /// <summary>
    /// Definies the functiinality used by Nancy to located a view.
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
    }
}