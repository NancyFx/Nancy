namespace Nancy.ViewEngines.DotLiquid
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using Responses;
    using global::DotLiquid;
    using global::DotLiquid.FileSystems;

    public class DotLiquidViewEngine : IViewEngine
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DotLiquidViewEngine"/> class.
        /// </summary>
        public DotLiquidViewEngine()
            : this(new LiquidNancyFileSystem(string.Empty))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DotLiquidViewEngine"/> class.
        /// </summary>
        /// <param name="fileSystem"></param>
        public DotLiquidViewEngine(IFileSystem fileSystem)
        {
            if (fileSystem != null)
            {
                // TODO - Wrap around Nancy view locator / cache??
                Template.FileSystem = fileSystem;
            }
        }

        /// <summary>
        /// Gets the extensions file extensions that are supported by the view engine.
        /// </summary>
        /// <value>An <see cref="IEnumerable{T}"/> instance containing the extensions.</value>
        /// <remarks>The extensions should not have a leading dot in the name.</remarks>
        public IEnumerable<string> Extensions
        {
            get { yield return "liquid"; }
        }

        public void Initialize(ViewEngineStartupContext viewEngineStartupContext)
        {
        }

        /// <summary>
        /// Renders the view.
        /// </summary>
        /// <param name="viewLocationResult">A <see cref="ViewLocationResult"/> instance, containing information on how to get the view template.</param>
        /// <param name="model">The model that should be passed into the view</param>
        /// <param name="renderContext"></param>
        /// <returns>A response</returns>
        public Response RenderView(ViewLocationResult viewLocationResult, dynamic model, IRenderContext renderContext)
        {
            return new HtmlResponse(contents: stream =>
            {
                var hashedModel =
                    Hash.FromAnonymousObject(new { model = new DynamicDrop(model) });

                var parsed = renderContext.ViewCache.GetOrAdd(
                    viewLocationResult,
                    x => Template.Parse(viewLocationResult.Contents.Invoke().ReadToEnd()));

                var rendered = parsed.Render(hashedModel);

                var writer = new StreamWriter(stream);

                writer.Write(rendered);
                writer.Flush();
            });
        }
    }
}
