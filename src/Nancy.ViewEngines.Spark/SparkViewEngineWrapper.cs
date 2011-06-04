namespace Nancy.ViewEngines.Spark
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Dynamic;
    using System.IO;
    using System.Threading;
    using global::Spark;
    using global::Spark.Compiler;
    using global::Spark.FileSystem;
    using Nancy.ViewEngines.Spark.Descriptors;

    public class SparkViewEngineWrapper : IViewEngine
    {
        private readonly IRootPathProvider rootPathProvider;
        private IDescriptorBuilder descriptorBuilder;
        private ISparkViewEngine engine;

        public SparkViewEngineWrapper(IRootPathProvider rootPathProvider)
        {
            this.rootPathProvider = rootPathProvider;
            this.Settings = (ISparkSettings) ConfigurationManager.GetSection("spark") ?? new SparkSettings();
            this.ViewFolder = new FileSystemViewFolder(Path.Combine(this.rootPathProvider.GetRootPath()));
        }

        public ISparkSettings Settings { get; set; }

        public ISparkViewEngine Engine
        {
            get
            {
                if (this.engine == null)
                {
                    this.SetEngine(new SparkViewEngine(this.Settings));
                }

                return this.engine;
            }
            set
            {
                this.SetEngine(value);
            }
        }

        public IViewFolder ViewFolder
        {
            get
            {
                return this.Engine.ViewFolder;
            }
            set
            {
                this.Engine.ViewFolder = value;
            }
        }

        public IDescriptorBuilder DescriptorBuilder
        {
            get
            {
                return this.descriptorBuilder ??
                    Interlocked.CompareExchange(ref this.descriptorBuilder, new DefaultDescriptorBuilder(this.Engine), null) ??
                        this.descriptorBuilder;
            }
            set
            {
                this.descriptorBuilder = value;
            }
        }

        public IRenderContext RenderContext { get; set; }

        public ViewLocationResult ViewLocationResult { get; set; }

        /// <summary>
        /// Gets the extensions file extensions that are supported by the view engine.
        /// </summary>
        /// <value>An <see cref="IEnumerable{T}"/> instance containing the extensions.</value>
        /// <remarks>The extensions should not have a leading dot in the name.</remarks>
        public IEnumerable<string> Extensions
        {
            get
            {
                yield return "spark";
            }
        }

        public Action<Stream> RenderView(ViewLocationResult viewLocationResult, dynamic model, IRenderContext renderContext)
        {
            return stream =>
                   {
                       SparkViewEngineResult sparkViewEngineResult =
                           this.RenderViewInternal(viewLocationResult, model ?? new ExpandoObject(), renderContext);

                       var writer =
                           new StreamWriter(stream);

                       sparkViewEngineResult.View.Writer = writer;
                       sparkViewEngineResult.View.Model = model;
                       sparkViewEngineResult.View.Execute();

                       writer.Flush();
                   };
        }

        public void SetEngine(ISparkViewEngine viewEngine)
        {
            this.descriptorBuilder = null;
            this.engine = viewEngine;

            if (this.engine != null)
            {
                this.engine.DefaultPageBaseType = typeof (NancySparkView).FullName;
            }
        }

        private SparkViewEngineResult FindViewInternal(string viewPath, string viewName, string masterName, bool findDefaultMaster,
            IDictionary<string, object> extraParams)
        {
            var searchedLocations = new List<string>();

            var descriptorParams = new BuildDescriptorParams(
                viewPath,
                viewName,
                masterName,
                findDefaultMaster,
                extraParams);

            SparkViewDescriptor descriptor = this.DescriptorBuilder.BuildDescriptor(
                descriptorParams,
                searchedLocations);

            if (descriptor == null)
            {
                return new SparkViewEngineResult(searchedLocations);
            }

            var entry = (ISparkViewEntry) this.RenderContext.ViewCache.Retrieve(this.ViewLocationResult);

            if (entry == null)
            {
                entry = this.Engine.CreateEntry(descriptor);
                this.RenderContext.ViewCache.Store(this.ViewLocationResult, entry);
            }

            ISparkView sparkView = entry.CreateInstance();
            return new SparkViewEngineResult(sparkView as NancySparkView, this);
        }

        public SparkViewDescriptor CreateDescriptor(
            ViewLocationResult viewLocationResult,
            string masterName,
            bool findDefaultMaster,
            ICollection<string> searchedLocations)
        {
            return this.DescriptorBuilder.BuildDescriptor(
                new BuildDescriptorParams(
                    viewLocationResult.Location,
                    viewLocationResult.Name,
                    masterName,
                    findDefaultMaster,
                    this.DescriptorBuilder.GetExtraParameters(viewLocationResult)),
                searchedLocations);
        }

        private SparkViewEngineResult RenderViewInternal<TModel>(ViewLocationResult viewLocationResult, TModel model, IRenderContext renderContext)
        {
            this.RenderContext = renderContext;
            this.ViewLocationResult = viewLocationResult;

            SparkViewEngineResult result = this.FindViewInternal(
                Path.GetDirectoryName(viewLocationResult.Location), Path.GetFileNameWithoutExtension(viewLocationResult.Name), null, true, null);

            var viewWithModel = result.View as NancySparkView<TModel>;

            if (viewWithModel != null)
            {
                viewWithModel.SetModel(model);
            }

            return result;
        }
    }
}