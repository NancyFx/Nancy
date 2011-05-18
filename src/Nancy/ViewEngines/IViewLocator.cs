namespace Nancy.ViewEngines
{
    /// <summary>
    /// Defines the functionality for locating the requested view.
    /// </summary>
    public interface IViewLocator : IHideObjectMembers
    {
        /// <summary>
        /// Gets the location of the view defined by the <paramref name="viewName"/> parameter.
        /// </summary>
        /// <param name="viewName">Name of the view to locate.</param>
        /// <returns>A <see cref="ViewLocationResult"/> instance if the view could be located; otherwise <see langword="null"/>.</returns>
        ViewLocationResult LocateView(string viewName);
    }
}