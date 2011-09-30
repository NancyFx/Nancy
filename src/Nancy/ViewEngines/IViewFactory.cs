namespace Nancy.ViewEngines
{
    using System;
    using System.IO;

    /// <summary>
    /// Defines the functionality used by a <see cref="NancyModule"/> to render a view to the response.
    /// </summary>
    public interface IViewFactory : IHideObjectMembers
    {
        /// <summary>
        /// Renders the view with the name and model defined by the <paramref name="viewName"/> and <paramref name="model"/> parameters.
        /// </summary>
        /// <param name="viewName">The name of the view to render.</param>
        /// <param name="model">The module path of the module that is rendering the view.</param>
        /// <param name="viewLocationContext">A <see cref="ViewLocationContext"/> instance, containing information about the context for which the view is being rendered.</param>
        /// <returns>A response.</returns>
        Response RenderView(string viewName, dynamic model, ViewLocationContext viewLocationContext);
    }
}