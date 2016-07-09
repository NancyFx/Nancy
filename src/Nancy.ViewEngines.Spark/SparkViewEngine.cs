namespace Nancy.ViewEngines.Spark
{
    using System.Collections.Generic;
    using System.Configuration;
    using System.Dynamic;
    using System.IO;
    using Configuration;
    using global::Spark;
    using global::Spark.FileSystem;

    using Nancy.Responses;
    using Nancy.ViewEngines.Spark.Descriptors;

    /// <summary>
    /// View engine for rendering spark views.
    /// </summary>
    public class SparkViewEngine : IViewEngine
    {
        private readonly INancyEnvironment environment;
        private readonly IDescriptorBuilder descriptorBuilder;
        private readonly global::Spark.SparkViewEngine engine;
        private readonly ISparkSettings settings;
        private readonly string[] extensions = new[] { "spark", "shade" };

        /// <summary>
        /// Initializes a new instance of the <see cref="SparkViewEngine"/> class.
        /// </summary>
        public SparkViewEngine(IRootPathProvider rootPathProvider, INancyEnvironment environment)
        {
            this.environment = environment;
            this.settings = (ISparkSettings) ConfigurationManager.GetSection("spark") ?? new SparkSettings();

            this.engine =
                new global::Spark.SparkViewEngine(this.settings)
                {
                    DefaultPageBaseType = typeof (NancySparkView).FullName,
                    BindingProvider = new NancyBindingProvider(rootPathProvider),
                };

            this.descriptorBuilder = new DefaultDescriptorBuilder(this.engine);
        }

        /// <summary>
        /// Gets the extensions file extensions that are supported by the view engine.
        /// </summary>
        /// <value>An <see cref="IEnumerable{T}"/> instance containing the extensions.</value>
        /// <remarks>The extensions should not have a leading dot in the name.</remarks>
        public IEnumerable<string> Extensions
        {
            get { return this.extensions; }
        }

        private SparkViewEngineResult CreateView<TModel>(ViewLocationResult viewLocationResult, TModel model, IRenderContext renderContext)
        {
            var result = this.LocateView(
                viewLocationResult.Location,
                viewLocationResult.Name,
                viewLocationResult,
                renderContext);

            var viewWithModel = result.View;

            if (viewWithModel != null)
            {
                viewWithModel.SetModel(model);
            }

            return result;
        }

        private static IViewFolder GetViewFolder(ViewEngineStartupContext viewLocationResults, INancyEnvironment environment)
        {
            return new NancyViewFolder(viewLocationResults, environment);
        }

        private SparkViewEngineResult LocateView(string viewPath, string viewName, ViewLocationResult viewLocationResult, IRenderContext renderContext)
        {
            var searchedLocations = new List<string>();

            var descriptorParams = new BuildDescriptorParams(
                viewPath,
                viewName,
                null,
                true,
                null);

            var descriptor = this.descriptorBuilder.BuildDescriptor(
                descriptorParams,
                searchedLocations);

            if (descriptor == null)
            {
                return new SparkViewEngineResult(searchedLocations);
            }

            var entry = renderContext.ViewCache.GetOrAdd(
                viewLocationResult,
                x => this.engine.CreateEntry(descriptor));

            var nancySparkView = entry.CreateInstance() as NancySparkView;
            if (nancySparkView != null)
            {
                nancySparkView.RenderContext = renderContext;
            }

            return new SparkViewEngineResult(nancySparkView);
        }

        /// <summary>
        /// Initialise the view engine (if necessary)
        /// </summary>
        /// <param name="viewEngineStartupContext">Startup context</param>
        public void Initialize(ViewEngineStartupContext viewEngineStartupContext)
        {
            this.engine.ViewFolder = GetViewFolder(viewEngineStartupContext, this.environment);
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
                var sparkRenderConext = new SparkRenderContextWrapper(renderContext, engine);

                SparkViewEngineResult sparkViewEngineResult =
                    this.CreateView(viewLocationResult, model ?? new ExpandoObject(), sparkRenderConext);

                var writer =
                    new StreamWriter(stream);

                sparkViewEngineResult.View.Writer = writer;
                sparkViewEngineResult.View.Model = model;
                sparkViewEngineResult.View.Execute();

                writer.Flush();
            });
        }
    }
}
