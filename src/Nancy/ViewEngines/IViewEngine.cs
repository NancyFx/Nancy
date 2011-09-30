namespace Nancy.ViewEngines
{
    using System;
    using System.Collections.Generic;
    using System.IO;

    /// <summary>
    /// Defines the functionality that a view engine must support to be integrated into Nancy.
    /// </summary>
    public interface IViewEngine
    {
        /// <summary>
        /// Gets the extensions file extensions that are supported by the view engine.
        /// </summary>
        /// <value>An <see cref="IEnumerable{T}"/> instance containing the extensions.</value>
        /// <remarks>The extensions should not have a leading dot in the name.</remarks>
        IEnumerable<string> Extensions { get; }

        /// <summary>
        /// Initialise the view engine (if necessary)
        /// </summary>
        /// <param name="viewEngineStartupContext">Startup context</param>
        void Initialize(ViewEngineStartupContext viewEngineStartupContext);

        /// <summary>
        /// Renders the view.
        /// </summary>
        /// <param name="viewLocationResult">A <see cref="ViewLocationResult"/> instance, containing information on how to get the view template.</param>
        /// <param name="model">The model that should be passed into the view</param>
        /// <param name="renderContext"></param>
        /// <returns>A response</returns>
        Response RenderView(ViewLocationResult viewLocationResult, dynamic model, IRenderContext renderContext);
    }
}