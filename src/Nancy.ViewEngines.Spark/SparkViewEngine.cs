namespace Nancy.ViewEngines.Spark
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Dynamic;
    using System.IO;
    using Responses;
    using global::Spark;
    using global::Spark.FileSystem;
    using Nancy.ViewEngines.Spark.Descriptors;

    /// <summary>
    /// View engine for rendering spark views.
    /// </summary>
    public class SparkViewEngine : IViewEngine
    {
        private readonly IDescriptorBuilder descriptorBuilder;
        private readonly ISparkViewEngine engine;
        private readonly ISparkSettings settings;

        /// <summary>
        /// Initializes a new instance of the <see cref="SparkViewEngine"/> class.
        /// </summary>
        public SparkViewEngine()
        {
            this.settings = (ISparkSettings) ConfigurationManager.GetSection("spark") ?? new SparkSettings();

            this.engine = new global::Spark.SparkViewEngine(this.settings)
            {
                DefaultPageBaseType = typeof(NancySparkView).FullName
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
            get { yield return "spark"; }
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

        private static IViewFolder GetViewFolder(ViewEngineStartupContext viewLocationResults)
        {
            return new NancyViewFolder(viewLocationResults);
        }

        private static string GetViewFolderKey(ViewLocationResult viewLocationResult)
        {
            return string.Concat(GetNamespaceEncodedPathViewPath(viewLocationResult.Location), Path.DirectorySeparatorChar, viewLocationResult.Name, ".", viewLocationResult.Extension);
        }

        private SparkViewEngineResult LocateView(string viewPath, string viewName, ViewLocationResult viewLocationResult, IRenderContext renderContext)
        {
            var searchedLocations = new List<string>();

            var descriptorParams = new BuildDescriptorParams(
                GetNamespaceEncodedPathViewPath(viewPath),
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

        private static string GetNamespaceEncodedPathViewPath(string viewPath)
        {
            return viewPath.Replace('/', '_');
        }

        public void Initialize(ViewEngineStartupContext viewEngineStartupContext)
        {
            this.engine.ViewFolder = GetViewFolder(viewEngineStartupContext);
        }

        public Response RenderView(ViewLocationResult viewLocationResult, dynamic model, IRenderContext renderContext)
        {
            return new HtmlResponse(contents: stream =>
            {
                SparkViewEngineResult sparkViewEngineResult =
                    this.CreateView(viewLocationResult, model ?? new ExpandoObject(), renderContext);

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