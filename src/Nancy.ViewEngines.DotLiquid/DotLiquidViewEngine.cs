namespace Nancy.ViewEngines.DotLiquid {
    using Responses;
    using System;
    using System.Collections.Generic;
    using System.Dynamic;
    using System.IO;
    using global::DotLiquid;
    using global::DotLiquid.FileSystems;
    using global::DotLiquid.NamingConventions;
    using global::DotLiquid.Exceptions;

    /// <summary>
    /// View engine for rendering dotLiquid views.
    /// </summary>
    public class DotLiquidViewEngine : IViewEngine {
        private readonly IFileSystemFactory fileSystemFactory;

        /// <summary>
        /// Initializes a new instance of the <see cref="DotLiquidViewEngine"/> class.
        /// </summary>
        /// <remarks>The instance will use the <see cref="DefaultFileSystemFactory"/> internally.</remarks>
        public DotLiquidViewEngine()
            : this(new DefaultFileSystemFactory()) {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DotLiquidViewEngine"/> class.
        /// </summary>
        /// <param name="fileSystemFactory">Factory used to retrieve the <see cref="IFileSystem"/> instance that should be used by the engine.</param>
        public DotLiquidViewEngine(IFileSystemFactory fileSystemFactory) {
            this.fileSystemFactory = fileSystemFactory;
        }

        /// <summary>
        /// Gets the extensions file extensions that are supported by the view engine.
        /// </summary>
        /// <value>An <see cref="IEnumerable{T}"/> instance containing the extensions.</value>
        /// <remarks>The extensions should not have a leading dot in the name.</remarks>
        public IEnumerable<string> Extensions {
            get { yield return "liquid"; }
        }

        /// <summary>
        /// Initialise the view engine (if necessary)
        /// </summary>
        /// <param name="viewEngineStartupContext">Startup context</param>
        public void Initialize(ViewEngineStartupContext viewEngineStartupContext) {
            Template.FileSystem = this.fileSystemFactory.GetFileSystem(viewEngineStartupContext);
            Template.NamingConvention = new CSharpNamingConvention();
        }

        /// <summary>
        /// Renders the view.
        /// </summary>
        /// <param name="viewLocationResult">A <see cref="ViewLocationResult"/> instance, containing information on how to get the view template.</param>
        /// <param name="model">The model that should be passed into the view</param>
        /// <param name="renderContext"></param>
        /// <returns>A response</returns>
        public Response RenderView(ViewLocationResult viewLocationResult, dynamic model, IRenderContext renderContext) {
            try {
                // Parse the template
                var parsed = renderContext.ViewCache.GetOrAdd(
                    viewLocationResult,
                    x => Template.Parse(viewLocationResult.Contents.Invoke().ReadToEnd()));

                // Construct the response
                return new HtmlResponse(contents: stream => {
                    var hashedModel =
                        Hash.FromAnonymousObject(new {
                            Model = new DynamicDrop(model),
                            ViewBag = new DynamicDrop(renderContext.Context.ViewBag)
                        });

                    parsed.Render(stream, new RenderParameters {
                        LocalVariables = hashedModel,
                        Registers = Hash.FromAnonymousObject(new { nancy = renderContext })
                    });
                });
            }
            catch (global::DotLiquid.Exceptions.SyntaxException syntaxException) {
                // If parsing fails due to a template rendering exception, bring up this ViewEngine's `InternalServerError` page
                return new HtmlResponse(HttpStatusCode.InternalServerError, contents: stream => {
                    // Create the error model with a Nancy DynamicDictionary because i can ;)
                    DynamicDictionary errorModel = new DynamicDictionary();
                    errorModel.Add(new KeyValuePair<string, dynamic>("ErrorMessage", syntaxException.Message));

                    // Hash up the Error model so DotLiquid will understand it
                    var hashedErrorModel =
                        Hash.FromAnonymousObject(new {
                            Model = new DynamicDrop(errorModel)
                        });

                    // Grab the error HTML from the embedded resource
                    String errorHtml = LoadResource(@"500.liquid");

                    Template.Parse(errorHtml).Render(stream, new RenderParameters {
                        LocalVariables = hashedErrorModel
                    });
                });
            }
        }

        private static string LoadResource(string filename) {
            var resourceStream = typeof(DotLiquidViewEngine).Assembly.GetManifestResourceStream(String.Format("Nancy.ViewEngines.DotLiquid.Resources.{0}", filename));

            if (resourceStream == null) {
                return string.Empty;
            }

            using (var reader = new StreamReader(resourceStream)) {
                return reader.ReadToEnd();
            }
        }
    }
}
