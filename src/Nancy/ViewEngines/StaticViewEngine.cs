namespace Nancy.ViewEngines
{
    using System;
    using System.Collections.Generic;
    using System.IO;

    /// <summary>
    /// View engine for rendering static html files.
    /// </summary>
    public class StaticViewEngine : IViewEngine
    {
        /// <summary>
        /// Gets the extensions file extensions that are supported by the view engine.
        /// </summary>
        /// <value>An <see cref="IEnumerable{T}"/> instance containing the extensions.</value>
        /// <remarks>The extensions should not have a leading dot in the name.</remarks>
        public IEnumerable<string> Extensions
        {
            get { return new[] { "html", "htm" }; }
        }

        /// <summary>
        /// Renders the view.
        /// </summary>
        /// <param name="viewLocationResult">A <see cref="ViewLocationResult"/> instance, containing information on how to get the view template.</param>
        /// <param name="model">The model that should be passed into the view</param>
        /// <returns>A delegate that can be invoked with the <see cref="Stream"/> that the view should be rendered to.</returns>
        public Action<Stream> RenderView(ViewLocationResult viewLocationResult, dynamic model)
        {
            return stream =>
            {
                using (var writer = new StreamWriter(stream))
                {
                    writer.Write(viewLocationResult.Contents.ReadToEnd());
                    writer.Flush();
                }
            };
        }
    }
}