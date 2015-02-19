namespace Nancy.ViewEngines.DotLiquid
{
    using System;
    using System.Collections.Generic;
    using System.IO;

    using global::DotLiquid;
    using global::DotLiquid.Exceptions;
    using global::DotLiquid.FileSystems;
    using global::DotLiquid.NamingConventions;

    using Nancy.Responses;

    /// <summary>
    /// View engine for rendering dotLiquid views.
    /// </summary>
    public class DotLiquidViewEngine : IViewEngine
    {
        private readonly IFileSystemFactory fileSystemFactory;
        private readonly INamingConvention namingConvention;

        /// <summary>
        /// Initializes a new instance of the <see cref="DotLiquidViewEngine"/> class.
        /// </summary>
        /// <param name="namingConvention">Determines the DotLiquid naming convention that will be used for filters and Drops. This will default to the <c>RubyNamingConvention</c>.</param>
        /// <remarks>The instance will use the <see cref="DefaultFileSystemFactory"/> internally.</remarks>
        public DotLiquidViewEngine(INamingConvention namingConvention)
            : this(new DefaultFileSystemFactory(), namingConvention)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DotLiquidViewEngine"/> class.
        /// </summary>
        /// <param name="fileSystemFactory">Factory used to retrieve the <see cref="IFileSystem"/> instance that should be used by the engine.</param>
        /// <param name="namingConvention">The naming convention used by filters and DotLiquid's <c>Drop</c>s</param>
        public DotLiquidViewEngine(IFileSystemFactory fileSystemFactory, INamingConvention namingConvention)
        {
            this.fileSystemFactory = fileSystemFactory;
            this.namingConvention = namingConvention;
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
            Template.FileSystem = this.fileSystemFactory.GetFileSystem(viewEngineStartupContext, this.Extensions);
            Template.NamingConvention = this.namingConvention;
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
            Template parsed;
            Hash hashedModel;
            HttpStatusCode status;

            try
            {
                // Set the parsed template
                parsed = renderContext.ViewCache.GetOrAdd(
                    viewLocationResult,
                    x =>
                    {
                        using (var reader = viewLocationResult.Contents.Invoke())
                            return Template.Parse(reader.ReadToEnd());
                    });

                hashedModel = Hash.FromAnonymousObject(new
                {
                    Model = new DynamicDrop(model),
                    ViewBag = new DynamicDrop(renderContext.Context.ViewBag)
                });

                // If we got past that, we have a good response
                status = HttpStatusCode.OK;

            }
            catch (SyntaxException syntaxException)
            {
                // Syntax Exceptions cause a 500
                status = HttpStatusCode.InternalServerError;

                // Build the error message
                String errorMessage = String.Format("Syntax error in liquid view '{0}':\r\n\r\n{1}",
                    String.Format("{0}/{1}.{2}", viewLocationResult.Location, viewLocationResult.Name, viewLocationResult.Extension),
                    syntaxException.Message);

                // Create the error model with a Nancy DynamicDictionary because i can ;)
                DynamicDictionary errorModel = new DynamicDictionary();
                errorModel.Add(new KeyValuePair<string, dynamic>("ErrorMessage", errorMessage));

                // Hash up the Error model so DotLiquid will understand it
                hashedModel =
                    Hash.FromAnonymousObject(new
                    {
                        Model = new DynamicDrop(errorModel)
                    });

                // Grab the error HTML from the embedded resource and build up the DotLiquid template.
                String errorHtml = LoadResource(@"500.liquid");
                parsed = Template.Parse(errorHtml);
            }
            catch (Exception ex)
            {
                status = HttpStatusCode.InternalServerError;
                // Build the error message
                String errorMessage = String.Format("Error: {0}", ex.Message);

                // Create the error model with a Nancy DynamicDictionary because i can ;)
                DynamicDictionary errorModel = new DynamicDictionary();
                errorModel.Add(new KeyValuePair<string, dynamic>("ErrorMessage", errorMessage));

                // Hash up the Error model so DotLiquid will understand it
                hashedModel =
                    Hash.FromAnonymousObject(new
                    {
                        Model = new DynamicDrop(errorModel)
                    });

                // Grab the error HTML from the embedded resource
                String errorHtml = LoadResource(@"500.liquid");
                parsed = Template.Parse(errorHtml);
            }

            // Build the response
            return new HtmlResponse(statusCode: status, contents: stream =>
            {
                parsed.Render(stream, new RenderParameters
                {
                    LocalVariables = hashedModel,
                    Registers = Hash.FromAnonymousObject(new { nancy = renderContext })
                });
            });
        }

        private static string LoadResource(string filename)
        {
            var resourceStream = typeof(DotLiquidViewEngine).Assembly.GetManifestResourceStream(String.Format("Nancy.ViewEngines.DotLiquid.Resources.{0}", filename));

            if (resourceStream == null)
            {
                return string.Empty;
            }

            using (var reader = new StreamReader(resourceStream))
            {
                return reader.ReadToEnd();
            }
        }
    }
}
