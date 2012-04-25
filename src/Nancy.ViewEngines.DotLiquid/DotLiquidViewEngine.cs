namespace Nancy.ViewEngines.DotLiquid
{
    using System.Collections.Generic;
    using System.IO;
    using global::DotLiquid.FileSystems;
    using Responses;
    using global::DotLiquid;

    /// <summary>
    /// View engine for rendering dotLiquid views.
    /// </summary>
    public class DotLiquidViewEngine : IViewEngine
    {
        private readonly IFileSystemFactory fileSystemFactory;

        /// <summary>
        /// Initializes a new instance of the <see cref="DotLiquidViewEngine"/> class.
        /// </summary>
        /// <remarks>The instance will use the <see cref="DefaultFileSystemFactory"/> internally.</remarks>
        public DotLiquidViewEngine()
            : this(new DefaultFileSystemFactory())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DotLiquidViewEngine"/> class.
        /// </summary>
        /// <param name="fileSystemFactory">Factory used to retrieve the <see cref="IFileSystem"/> instance that should be used by the engine.</param>
        public DotLiquidViewEngine(IFileSystemFactory fileSystemFactory)
        {
            this.fileSystemFactory = fileSystemFactory;
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

        /// <summary>
        /// Initialise the view engine (if necessary)
        /// </summary>
        /// <param name="viewEngineStartupContext">Startup context</param>
        public void Initialize(ViewEngineStartupContext viewEngineStartupContext)
        {
            Template.FileSystem = this.fileSystemFactory.GetFileSystem(viewEngineStartupContext);
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

                parsed.Render(stream, new RenderParameters
                    {
                        LocalVariables = hashedModel,
                        Registers = Hash.FromAnonymousObject(new { nancy = renderContext })
                    });
            });
        }
    }
}
