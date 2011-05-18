namespace Nancy.ViewEngines
{
    /// <summary>
    /// Defines the functionality for resolving the requested view.
    /// </summary>
    public interface IViewResolver : IHideObjectMembers
    {
        /// <summary>
        /// Locates a view based on the provided information.
        /// </summary>
        /// <param name="viewName">The name of the view to locate.</param>
        /// <param name="model">The model that will be used with the view.</param>
        /// <param name="viewLocationContext">A <see cref="ViewLocationContext"/> instance, containing information about the context for which the view is being located.</param>
        /// <returns>A <see cref="ViewLocationResult"/> instance if the view could be found, otherwise <see langword="null"/>.</returns>
        ViewLocationResult GetViewLocation(string viewName, dynamic model, ViewLocationContext viewLocationContext);
    }
}