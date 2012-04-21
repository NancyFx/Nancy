namespace Nancy.ViewEngines.Razor
{
    using System;
    using System.CodeDom;
    using System.CodeDom.Compiler;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Text;
    using System.Web.Razor;
    using System.Web.Razor.Parser.SyntaxTree;

    using Nancy.Bootstrapper;
    using Nancy.Responses;

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
        /// <remarks>Well create an instance of the engine using the <see cref="DefaultRazorConfiguration"/>.</remarks>
        public RazorViewEngine()
            : this(new DefaultRazorConfiguration())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RazorViewEngine"/> class.
        /// </summary>
        /// <param name="configuration">The <see cref="IRazorConfiguration"/> that should be used by the engine.</param>
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
        {
        }

        /// <summary>
        /// Renders the view.
        /// </summary>
        /// <param name="viewLocationResult">A <see cref="ViewLocationResult"/> instance, containing information on how to get the view template.</param>
        /// <param name="model">The model that should be passed into the view</param>
        /// <param name="renderContext">The render context.</param>
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
                    layout = view.Layout;
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

        private Func<NancyRazorViewBase> GetCompiledViewFactory(string extension, TextReader reader, Assembly referencingAssembly, Type passedModelType)
        {
            var renderer = this.viewRenderers
                .Where(x => x.Extension.Equals(extension, StringComparison.OrdinalIgnoreCase))
                .First();

            var engine = this.GetRazorTemplateEngine(renderer.Host);
            
            var razorResult = engine.GenerateCode(reader);

            var viewFactory = this.GenerateRazorViewFactory(renderer.Provider, razorResult, referencingAssembly, renderer.Assemblies, passedModelType);

            return viewFactory;
        }

        private Func<NancyRazorViewBase> GenerateRazorViewFactory(CodeDomProvider codeProvider, GeneratorResults razorResult, Assembly referencingAssembly, IEnumerable<string> rendererSpecificAssemblies, Type passedModelType)
        {
            var outputAssemblyName = Path.Combine(Path.GetTempPath(), String.Format("Temp_{0}.dll", Guid.NewGuid().ToString("N")));

            var modelType = FindModelType(razorResult.Document, passedModelType);

            var assemblies = new List<string>
            {
                GetAssemblyPath(typeof(System.Runtime.CompilerServices.CallSite)),
                GetAssemblyPath(typeof(IHtmlString)),
                GetAssemblyPath(Assembly.GetExecutingAssembly()),
                GetAssemblyPath(modelType)
            };

            if (referencingAssembly != null)
            {
                assemblies.Add(GetAssemblyPath(referencingAssembly));
            }

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
                    AddModelNamespace(razorResult, modelType);
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

        private static Type FindModelType(Block block, Type passedModelType)
        {
            var modelFinder = new ModelFinder();
            block.Accept(modelFinder);

            if (string.IsNullOrWhiteSpace(modelFinder.ModelTypeName))
            {
                return passedModelType ?? typeof(object);
            }

            Type modelType;

            if (passedModelType != null)
            {
                modelType = passedModelType;
                while (modelType != null)
                {
                    if (modelType.FullName == modelFinder.ModelTypeName || modelType.Name == modelFinder.ModelTypeName)
                    {
                        return modelType;
                    }

                    modelType = modelType.BaseType;
                }

                throw new NotSupportedException(string.Format("Unable to discover CLR Type for model by the name of {0}.  Ensure that the model passed to the view is assignable to the model declared in the view.", modelFinder.ModelTypeName));
            }

            modelType = Type.GetType(modelFinder.ModelTypeName);

            if (modelType != null)
            {
                return modelType;
            }

            modelType = AppDomainAssemblyTypeScanner.Types.Where(t => t.FullName == modelFinder.ModelTypeName).FirstOrDefault();

            if (modelType != null)
            {
                return modelType;
            }

            modelType = AppDomainAssemblyTypeScanner.Types.Where(t => t.Name == modelFinder.ModelTypeName).FirstOrDefault();

            if (modelType != null)
            {
                return modelType;
            }

            throw new NotSupportedException(string.Format("Unable to discover CLR Type for model by the name of {0}. Try using a fully qualified type name and ensure that the assembly is added to the configuration file.", modelFinder.ModelTypeName));
        }

        private static void AddModelNamespace(GeneratorResults razorResult, Type modelType)
        {
            if (razorResult.GeneratedCode.Namespaces[0].Imports.OfType<CodeNamespaceImport>().Any(x => x.Namespace == modelType.Namespace))
            {
                return;
            }

            razorResult.GeneratedCode.Namespaces[0].Imports.Add(new CodeNamespaceImport(modelType.Namespace));
        }

        private static string GetAssemblyPath(Type type)
        {
            return GetAssemblyPath(type.Assembly);
        }

        private static string GetAssemblyPath(Assembly assembly)
        {
            return new Uri(assembly.EscapedCodeBase).LocalPath;
        }

        private NancyRazorViewBase GetOrCompileView(ViewLocationResult viewLocationResult, IRenderContext renderContext, Assembly referencingAssembly, Type passedModelType)
        {
            var viewFactory = renderContext.ViewCache.GetOrAdd(
                viewLocationResult,
                x => this.GetCompiledViewFactory(x.Extension, x.Contents.Invoke(), referencingAssembly, passedModelType));

            var view = viewFactory.Invoke();

            view.Code = string.Empty;

            return view;
        }

        private NancyRazorViewBase GetViewInstance(ViewLocationResult viewLocationResult, IRenderContext renderContext, Assembly referencingAssembly, dynamic model)
        {
            var modelType = (model == null) ? null :  model.GetType();

            var view = 
                this.GetOrCompileView(viewLocationResult, renderContext, referencingAssembly, modelType);

            view.Initialize(this, renderContext, model);

            return view;
        }
    }
}