namespace Nancy.ViewEngines.SuperSimpleViewEngine
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Threading;
    using Responses;

    /// <summary>
    /// Nancy IViewEngine wrapper for the super simple view engine
    /// </summary>
    public class SuperSimpleViewEngineWrapper : IViewEngine
    {
        /// <summary>
        /// Extensions that the view engine supports
        /// </summary>
        private readonly string[] extensions = new[] { "sshtml", "html", "htm" };

        /// <summary>
        /// The engine itself
        /// </summary>
        private readonly SuperSimpleViewEngine viewEngine = new SuperSimpleViewEngine();

        /// <summary>
        /// Gets the extensions file extensions that are supported by the view engine.
        /// </summary>
        /// <value>An <see cref="IEnumerable{T}"/> instance containing the extensions.</value>
        /// <remarks>The extensions should not have a leading dot in the name.</remarks>
        public IEnumerable<string> Extensions
        {
            get { return this.extensions; }
        }

        public void Initialize(ViewEngineStartupContext viewEngineStartupContext)
        {
        }

        /// <summary>
        /// Renders the view.
        /// </summary>
        /// <param name="viewLocationResult">A <see cref="ViewLocationResult"/> instance, containing information on how to get the view template.</param>
        /// <param name="model">The model that should be passed into the view</param>
        /// <returns>A response.</returns>
        public Response RenderView(ViewLocationResult viewLocationResult, dynamic model, IRenderContext renderContext)
        {
            return new HtmlResponse(contents: s =>
                {
                    var writer = new StreamWriter(s);
                    var templateContents = renderContext.ViewCache.GetOrAdd(viewLocationResult, vr => vr.Contents.Invoke().ReadToEnd());

                    writer.Write(this.viewEngine.Render(templateContents, model, new NancyViewEngineHost(renderContext)));
                    writer.Flush();
                });
        }
    }
}