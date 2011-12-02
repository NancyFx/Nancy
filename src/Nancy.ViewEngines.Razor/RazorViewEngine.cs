namespace Nancy.ViewEngines.Razor
{
    using System;
    using System.CodeDom.Compiler;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Web;
    using System.Web.Razor;
    using Responses;

    /// <summary>
    /// View engine for rendering razor views.
    /// </summary>
    public class RazorViewEngine : IViewEngine
    {
        private readonly IRazorConfiguration razorConfiguration;
        private readonly IEnumerable<IRazorViewRenderer> viewRenderers;

        /// <summary>
        /// Gets the extensions file extensions that are supported by the view engine.
        /// </summary>
        /// <value>An <see cref="IEnumerable{T}"/> instance containing the extensions.</value>
        /// <remarks>The extensions should not have a leading dot in the name.</remarks>
        public IEnumerable<string> Extensions
        {
            get { return this.viewRenderers.Select(x => x.Extension); }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RazorViewEngine"/> class with a default configuration.
        /// </summary>
        public RazorViewEngine()
            : this(new DefaultRazorConfiguration())
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="RazorViewEngine"/> class.
        /// </summary>
        /// <param name="configuration"></param>
        public RazorViewEngine(IRazorConfiguration configuration)
        {
            this.viewRenderers = new List<IRazorViewRenderer>
            {
                new CSharp.CSharpRazorViewRenderer(),
                new VisualBasic.VisualBasicRazorViewRenderer()
            };

            this.razorConfiguration = configuration;
        }

        /// <summary>
        /// Initialise the view engine (if necessary)
        /// </summary>
        /// <param name="viewEngineStartupContext">Startup context</param>
        public void Initialize(ViewEngineStartupContext viewEngineStartupContext)
        { }

        /// <summary>
        /// Renders the view.
        /// </summary>
        /// <param name="viewLocationResult">A <see cref="ViewLocationResult"/> instance, containing information on how to get the view template.</param>
        /// <param name="model">The model that should be passed into the view</param>
        /// <param name="renderContext"></param>
        /// <returns>A response.</returns>
        public Response RenderView(ViewLocationResult viewLocationResult, dynamic model, IRenderContext renderContext)
        {
            Assembly referencingAssembly = null;

            if (model != null)
            {
                var underlyingSystemType = model.GetType().UnderlyingSystemType;
                if (underlyingSystemType != null)
                {
                    referencingAssembly = Assembly.GetAssembly(underlyingSystemType);
                }
            }

            var response = new HtmlResponse();

            response.Contents = stream =>
            {
                var writer = new StreamWriter(stream);
                var view = this.GetViewInstance(viewLocationResult, renderContext, referencingAssembly, model);
                view.ExecuteView(null, null);
                var body = view.Body;
                var sectionContents = view.SectionContents;
                var root = !view.HasLayout;
                var layout = view.Layout;

                while (!root)
                {
                    view = this.GetViewInstance(renderContext.LocateView(layout, model), renderContext, referencingAssembly, model);
                    view.ExecuteView(body, sectionContents);

                    body = view.Body;
                    sectionContents = view.SectionContents;
                    root = !view.HasLayout;
                }

                writer.Write(body);
                writer.Flush();
            };

            return response;
        }

        private RazorTemplateEngine GetRazorTemplateEngine(RazorEngineHost engineHost)
        {
            engineHost.NamespaceImports.Add("System");
            engineHost.NamespaceImports.Add("System.IO");
            engineHost.NamespaceImports.Add("System.Web");

            if (this.razorConfiguration != null)
            {
                var namespaces = this.razorConfiguration.GetDefaultNamespaces();
                if (namespaces != null)
                {
                    foreach (var n in namespaces)
                    {
                        engineHost.NamespaceImports.Add(n);
                    }
                }
            }

            return new RazorTemplateEngine(engineHost);
        }

        private Func<NancyRazorViewBase> GetCompiledViewFactory(string extension, TextReader reader, Assembly referencingAssembly)
        {
            var renderer = this.viewRenderers
                .Where(x => x.Extension.Equals(extension, StringComparison.OrdinalIgnoreCase))
                .First();

            var engine = this.GetRazorTemplateEngine(renderer.Host);

            var razorResult = engine.GenerateCode(reader);

            var viewFactory = this.GenerateRazorViewFactory(renderer.Provider, razorResult, referencingAssembly, renderer.Assemblies);

            return viewFactory;
        }

        private Func<NancyRazorViewBase> GenerateRazorViewFactory(CodeDomProvider codeProvider, GeneratorResults razorResult, Assembly referencingAssembly, IEnumerable<string> rendererSpecificAssemblies)
        {
            var outputAssemblyName = Path.Combine(Path.GetTempPath(), String.Format("Temp_{0}.dll", Guid.NewGuid().ToString("N")));

            var assemblies = new List<string>
            {
                GetAssemblyPath(typeof(System.Runtime.CompilerServices.CallSite)),
                GetAssemblyPath(typeof(IHtmlString)),
                GetAssemblyPath(Assembly.GetExecutingAssembly())
            };

            if (referencingAssembly != null)
                assemblies.Add(GetAssemblyPath(referencingAssembly));

            assemblies = assemblies
                .Union(rendererSpecificAssemblies)
                .ToList();

            if (this.razorConfiguration != null)
            {
                var assemblyNames = this.razorConfiguration.GetAssemblyNames();
                if (assemblyNames != null)
                {
                    assemblies.AddRange(assemblyNames.Select(Assembly.Load).Select(GetAssemblyPath));
                }

                if (this.razorConfiguration.AutoIncludeModelNamespace)
                {
                    //TODO: include model namespace...  this is actually quite hard...
                }
            }


            var compilerParameters = new CompilerParameters(assemblies.ToArray(), outputAssemblyName);

            var results = codeProvider.CompileAssemblyFromDom(compilerParameters, razorResult.GeneratedCode);

            if (results.Errors.HasErrors)
            {
                var err = results.Errors
                    .OfType<CompilerError>()
                    .Where(ce => !ce.IsWarning)
                    .Select(error => String.Format("Error Compiling Template: ({0}, {1}) {2})", error.Line, error.Column, error.ErrorText))
                    .Aggregate((s1, s2) => s1 + "<br/>" + s2);

                return () => new NancyRazorErrorView(err);
            }

            var assembly = Assembly.LoadFrom(outputAssemblyName);
            if (assembly == null)
            {
                const string error = "Error loading template assembly";
                return () => new NancyRazorErrorView(error);
            }

            var type = assembly.GetType("RazorOutput.RazorView");
            if (type == null)
            {
                var error = String.Format("Could not find type RazorOutput.Template in assembly {0}", assembly.FullName);
                return () => new NancyRazorErrorView(error);
            }

            if (Activator.CreateInstance(type) as NancyRazorViewBase == null)
            {
                const string error = "Could not construct RazorOutput.Template or it does not inherit from RazorViewBase";
                return () => new NancyRazorErrorView(error);
            }

            return () => (NancyRazorViewBase)Activator.CreateInstance(type);
        }

        private static string GetAssemblyPath(Type type)
        {
            return GetAssemblyPath(type.Assembly);
        }

        private static string GetAssemblyPath(Assembly assembly)
        {
            return new Uri(assembly.EscapedCodeBase).LocalPath;
        }

        private NancyRazorViewBase GetOrCompileView(ViewLocationResult viewLocationResult, IRenderContext renderContext, Assembly referencingAssembly)
        {
            var viewFactory = renderContext.ViewCache.GetOrAdd(
                viewLocationResult,
                x => this.GetCompiledViewFactory(x.Extension, x.Contents.Invoke(), referencingAssembly));

            var view = viewFactory.Invoke();

            view.Code = string.Empty;

            return view;
        }

        private NancyRazorViewBase GetViewInstance(ViewLocationResult viewLocationResult, IRenderContext renderContext, Assembly referencingAssembly, dynamic model)
        {
            var view = this.GetOrCompileView(viewLocationResult, renderContext, referencingAssembly);
            view.Initialize(this, renderContext, model);
            return view;
        }
    }
}