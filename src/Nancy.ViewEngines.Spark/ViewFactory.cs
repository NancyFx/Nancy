using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Web;
using System.Web.Hosting;
using Nancy.ViewEngines.Spark.Caching;
using Nancy.ViewEngines.Spark.Descriptors;
using Spark;
using Spark.Compiler;
using Spark.FileSystem;

namespace Nancy.ViewEngines.Spark
{
    public class ViewFactory : ISparkServiceInitialize
    {
        private readonly Dictionary<BuildDescriptorParams, ISparkViewEntry> cache =
            new Dictionary<BuildDescriptorParams, ISparkViewEntry>();

        private readonly ViewEngineResult cacheMissResult = new ViewEngineResult(new List<string>());
        private ICacheServiceProvider cacheServiceProvider;
        private IDescriptorBuilder descriptorBuilder;
        private ISparkViewEngine engine;


        public ViewFactory()
            : this(null)
        {
        }

        public ViewFactory(ISparkSettings settings)
        {
            Settings = settings ?? (ISparkSettings)ConfigurationManager.GetSection("spark") ?? new SparkSettings();
        }


        public ISparkSettings Settings { get; set; }

        public ISparkViewEngine Engine
        {
            get
            {
                if (engine == null)
                    SetEngine(new SparkViewEngine(Settings));

                return engine;
            }
            set { SetEngine(value); }
        }

        public IViewActivatorFactory ViewActivatorFactory
        {
            get { return Engine.ViewActivatorFactory; }
            set { Engine.ViewActivatorFactory = value; }
        }

        public IViewFolder ViewFolder
        {
            get { return Engine.ViewFolder; }
            set { Engine.ViewFolder = value; }
        }

        public IDescriptorBuilder DescriptorBuilder
        {
            get
            {
                return descriptorBuilder ??
                       Interlocked.CompareExchange(ref descriptorBuilder, new DefaultDescriptorBuilder(Engine), null) ??
                       descriptorBuilder;
            }
            set { descriptorBuilder = value; }
        }

        public ICacheServiceProvider CacheServiceProvider
        {
            get
            {
                return cacheServiceProvider ??
                       Interlocked.CompareExchange(ref cacheServiceProvider, new DefaultCacheServiceProvider(), null) ??
                       cacheServiceProvider;
            }
            set { cacheServiceProvider = value; }
        }

        #region ISparkServiceInitialize Members

        void ISparkServiceInitialize.Initialize(ISparkServiceContainer container)
        {
            Initialize(container);
        }

        #endregion

        public virtual void Initialize(ISparkServiceContainer container)
        {
            Settings = container.GetService<ISparkSettings>();
            Engine = container.GetService<ISparkViewEngine>();
            DescriptorBuilder = container.GetService<IDescriptorBuilder>();
            CacheServiceProvider = container.GetService<ICacheServiceProvider>();
        }

        public void SetEngine(ISparkViewEngine engine)
        {
            descriptorBuilder = null;
            this.engine = engine;
            if (this.engine != null)
            {
                this.engine.DefaultPageBaseType = typeof(SparkView).FullName;
            }
        }

        public virtual ViewEngineResult FindView(ActionContext actionContext, string viewName, string masterName)
        {
            return FindViewInternal(actionContext, viewName, masterName, true, false);
        }

        public virtual ViewEngineResult FindView(ActionContext actionContext, string viewName, string masterName, bool useCache)
        {
            return FindViewInternal(actionContext, viewName, masterName, true, useCache);
        }

        public virtual ViewEngineResult FindPartialView(ActionContext actionContext, string partialViewName)
        {
            return FindViewInternal(actionContext, partialViewName, null /*masterName*/, false, false);
        }

        public virtual ViewEngineResult FindPartialView(ActionContext actionContext, string partialViewName, bool useCache)
        {
            return FindViewInternal(actionContext, partialViewName, null /*masterName*/, false, useCache);
        }

        public virtual void ReleaseView(ActionContext actionContext, ISparkView view)
        {
            Engine.ReleaseInstance(view);
        }

        private ViewEngineResult FindViewInternal(ActionContext actionContext, string viewName, string masterName, bool findDefaultMaster, bool useCache)
        {
            var searchedLocations = new List<string>();
            string viewPath = actionContext.ViewPath;

            var descriptorParams = new BuildDescriptorParams(
                viewPath,
                viewName,
                masterName,
                findDefaultMaster,
                DescriptorBuilder.GetExtraParameters(actionContext));

            ISparkViewEntry entry;
            if (useCache)
            {
                if (TryGetCacheValue(descriptorParams, out entry) && entry.IsCurrent())
                    return BuildResult(actionContext.HttpContext, entry);
                return cacheMissResult;
            }

            SparkViewDescriptor descriptor = DescriptorBuilder.BuildDescriptor(
                descriptorParams,
                searchedLocations);

            if (descriptor == null)
                return new ViewEngineResult(searchedLocations);

            entry = Engine.CreateEntry(descriptor);
            SetCacheValue(descriptorParams, entry);
            return BuildResult(actionContext.HttpContext, entry);
        }

        private bool TryGetCacheValue(BuildDescriptorParams descriptorParams, out ISparkViewEntry entry)
        {
            lock (cache) return cache.TryGetValue(descriptorParams, out entry);
        }

        private void SetCacheValue(BuildDescriptorParams descriptorParams, ISparkViewEntry entry)
        {
            lock (cache) cache[descriptorParams] = entry;
        }


        private ViewEngineResult BuildResult(HttpContextBase httpContext, ISparkViewEntry entry)
        {
            ISparkView view = entry.CreateInstance();
            if (view is SparkView)
                ((SparkView)view).CacheService = CacheServiceProvider.GetCacheService(httpContext);
            return new ViewEngineResult(view, this);
        }

        public SparkViewDescriptor CreateDescriptor(
            ActionContext actionContext,
            string viewName,
            string masterName,
            bool findDefaultMaster,
            ICollection<string> searchedLocations)
        {
            string viewPath = actionContext.ViewPath;

            return DescriptorBuilder.BuildDescriptor(
                new BuildDescriptorParams(
                    viewPath,
                    viewName,
                    masterName,
                    findDefaultMaster,
                    DescriptorBuilder.GetExtraParameters(actionContext)),
                searchedLocations);
        }

        public SparkViewDescriptor CreateDescriptor(string viewPath, string controllerName, string viewName,
                                                    string masterName, bool findDefaultMaster)
        {
            var searchedLocations = new List<string>();
            SparkViewDescriptor descriptor = DescriptorBuilder.BuildDescriptor(
                new BuildDescriptorParams(
                    viewPath,
                    viewName,
                    masterName,
                    findDefaultMaster, null),
                searchedLocations);

            if (descriptor == null)
            {
                throw new CompilerException(
                    "Unable to find templates at " + string.Join(", ", searchedLocations.ToArray()));
            }
            return descriptor;
        }


        public Assembly Precompile(SparkBatchDescriptor batch)
        {
            return Engine.BatchCompilation(batch.OutputAssembly, CreateDescriptors(batch));
        }

        public List<SparkViewDescriptor> CreateDescriptors(SparkBatchDescriptor batch)
        {
            var descriptors = new List<SparkViewDescriptor>();
            foreach (SparkBatchEntry entry in batch.Entries)
                descriptors.AddRange(CreateDescriptors(entry));
            return descriptors;
        }

        public IList<SparkViewDescriptor> CreateDescriptors(SparkBatchEntry entry)
        {
            var descriptors = new List<SparkViewDescriptor>();

            string controllerName = RemoveSuffix(entry.ControllerType.Name, "Controller");

            var viewNames = new List<string>();
            IList<string> includeViews = entry.IncludeViews;
            if (includeViews.Count == 0)
                includeViews = new[] { "*" };

            foreach (string include in includeViews)
            {
                if (include.EndsWith("*"))
                {
                    foreach (string fileName in ViewFolder.ListViews(controllerName))
                    {
                        if (!string.Equals(Path.GetExtension(fileName), ".spark", StringComparison.InvariantCultureIgnoreCase))
                            continue;

                        string potentialMatch = Path.GetFileNameWithoutExtension(fileName);
                        if (!TestMatch(potentialMatch, include))
                            continue;

                        bool isExcluded = false;
                        foreach (string exclude in entry.ExcludeViews)
                        {
                            if (!TestMatch(potentialMatch, RemoveSuffix(exclude, ".spark")))
                                continue;

                            isExcluded = true;
                            break;
                        }
                        if (!isExcluded)
                            viewNames.Add(potentialMatch);
                    }
                }
                else
                {
                    // explicitly included views don't test for exclusion
                    viewNames.Add(RemoveSuffix(include, ".spark"));
                }
            }

            foreach (string viewName in viewNames)
            {
                if (entry.LayoutNames.Count == 0)
                {
                    descriptors.Add(CreateDescriptor(
                        entry.ControllerType.Namespace,
                        controllerName,
                        viewName,
                        null /*masterName*/,
                        true));
                }
                else
                {
                    foreach (var masterName in entry.LayoutNames)
                    {
                        descriptors.Add(CreateDescriptor(
                            entry.ControllerType.Namespace,
                            controllerName,
                            viewName,
                            string.Join(" ", masterName.ToArray()),
                            false));
                    }
                }
            }

            return descriptors;
        }

        private static bool TestMatch(string potentialMatch, string pattern)
        {
            if (!pattern.EndsWith("*"))
            {
                return string.Equals(potentialMatch, pattern, StringComparison.InvariantCultureIgnoreCase);
            }

            // raw wildcard matches anything that's not a partial
            if (pattern == "*")
            {
                return !potentialMatch.StartsWith("_");
            }

            // otherwise the only thing that's supported is "starts with"
            return potentialMatch.StartsWith(pattern.Substring(0, pattern.Length - 1),
                                             StringComparison.InvariantCultureIgnoreCase);
        }

        private static string RemoveSuffix(string value, string suffix)
        {
            if (value.EndsWith(suffix, StringComparison.InvariantCultureIgnoreCase))
                return value.Substring(0, value.Length - suffix.Length);
            return value;
        }

        #region Nested type: ViewEngineResult

        public class ViewEngineResult
        {
            public ViewEngineResult(ISparkView view, ViewFactory factory)
            {
                View = view;
                Factory = factory;
            }

            public ViewEngineResult(List<string> searchedLocations)
            {
                string locations = string.Empty;
                searchedLocations.ForEach(loc => locations += string.Format("{0} ", loc));
                if (!string.IsNullOrEmpty(locations))
                    throw new CompilerException(string.Format("The view could not be in any of the following locations: {0}", locations));
            }

            public ISparkView View { get; set; }
            public ViewFactory Factory { get; set; }
        }

        #endregion

        public ViewResult RenderView<TModel>(string path, TModel model)
        {
            var viewName = path.Substring(path.LastIndexOf('/') + 1).Replace(".spark", string.Empty);
            string viewPath = path.Substring(0, path.LastIndexOf('/'));
            string targetNamespace = string.Empty; //TODO Rob G: This can be used to support things like areas or features
            ViewFolder = new FileSystemViewFolder(HostingEnvironment.MapPath(viewPath));
            HttpContextBase httpContext = null; //TODO Rob G: figure out how to get httpcontext passed in so that we can support view and partial caching.
            var actionContext = new ActionContext(httpContext, targetNamespace);
            ViewEngineResult result = FindView(actionContext, viewName, null);
            var viewWithModel = result.View as SparkView<TModel>;
            if (viewWithModel != null)
                viewWithModel.SetModel(model);

            return new ViewResult(result.View as SparkView, HostingEnvironment.MapPath(path));
        }
    }
}