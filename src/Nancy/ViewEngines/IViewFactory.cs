namespace Nancy.ViewEngines
{
    using System;
    using System.IO;

    /// <summary>
    /// Defines the functionality used by a <see cref="NancyModule"/> to render a view to the response.
    /// </summary>
    public interface IViewFactory
    {
        /// <summary>
        /// Renders the view with the name defined by the <paramref name="viewName"/> parameter.
        /// </summary>
        /// <param name="viewName">The name of the view to render.</param>
        /// <returns>A delegate that can be invoked with the <see cref="Stream"/> that the view should be rendered to.</returns>
        Action<Stream> this[string viewName] { get; }

        /// <summary>
        /// Renders the view with the name and model defined by the <paramref name="viewName"/> and <paramref name="model"/> parameters.
        /// </summary>
        /// <param name="viewName">The name of the view to render.</param>
        /// <param name="model">The model that should be passed into the view.</param>
        /// <returns>A delegate that can be invoked with the <see cref="Stream"/> that the view should be rendered to.</returns>
        Action<Stream> this[string viewName, dynamic model] { get; }
    }
}