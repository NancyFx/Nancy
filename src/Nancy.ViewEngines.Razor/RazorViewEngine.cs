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
    using Microsoft.CSharp;

    /// <summary>
    /// View engine for rendering razor views.
    /// </summary>
    public class RazorViewEngine : IViewEngine
    {
        private readonly RazorTemplateEngine engine;
        private readonly CodeDomProvider codeDomProvider;
        private readonly IRazorConfiguration razorConfiguration;

        /// <summary>
        /// Initializes a new instance of the <see cref="RazorViewEngine"/> class with a default configuration.
        /// </summary>
        public RazorViewEngine() : this(new DefaultRazorConfiguration())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RazorViewEngine"/> class.
        /// </summary>
        /// <param name="configuration"></param>
        public RazorViewEngine(IRazorConfiguration configuration)
        {
            this.razorConfiguration = configuration;
            this.engine = this.GetRazorTemplateEngine();
            this.codeDomProvider = new CSharpCodeProvider();
        }

        private RazorTemplateEngine GetRazorTemplateEngine()
        {
            var host = 
                new RazorEngineHost(new CSharpRazorCodeLanguage())
                {
                    DefaultBaseClass = typeof(NancyRazorViewBase).FullName,
                    DefaultNamespace = "RazorOutput",
                    DefaultClassName = "RazorView"
                };

            host.NamespaceImports.Add("System");
            host.NamespaceImports.Add("System.IO");
            host.NamespaceImports.Add("System.Web");
            host.NamespaceImports.Add("Microsoft.CSharp.RuntimeBinder");

            if (this.razorConfiguration != null)
            {
                var namespaces = this.razorConfiguration.GetDefaultNamespaces();
                if (namespaces != null)
                {
                    foreach (var n in namespaces)
                    {
                        host.NamespaceImports.Add(n);
                    }
                }
            }

            return new RazorTemplateEngine(host);
        }

        private NancyRazorViewBase GetCompiledView<TModel>(TextReader reader, Assembly referencingAssembly) 
        {
            var razorResult = this.engine.GenerateCode(reader);

            //using (var sw = new StringWriter()) {
            //    this.codeDomProvider.GenerateCodeFromCompileUnit(razorResult.GeneratedCode, sw, new CodeGeneratorOptions());
            //    code = sw.GetStringBuilder().ToString();
            //}

            var view = 
                GenerateRazorView(this.codeDomProvider, razorResult, referencingAssembly);

            view.Code = string.Empty;

            return view;
        }

        private NancyRazorViewBase GenerateRazorView(CodeDomProvider codeProvider, GeneratorResults razorResult, Assembly referencingAssembly)
        {
            // Compile the generated code into an assembly
            var outputAssemblyName =
                Path.Combine(Path.GetTempPath(), String.Format("Temp_{0}.dll", Guid.NewGuid().ToString("N")));

            var assemblies = new List<string>
            {
                GetAssemblyPath(typeof(Microsoft.CSharp.RuntimeBinder.Binder)),
                GetAssemblyPath(typeof(System.Runtime.CompilerServices.CallSite)),
                GetAssemblyPath(typeof(IHtmlString)),
                GetAssemblyPath(Assembly.GetExecutingAssembly())
            };

            if (this.razorConfiguration != null)
            {
                var assemblyNames = this.razorConfiguration.GetAssemblyNames();
                if (assemblyNames != null)
                {
                    assemblies.AddRange(assemblyNames.Select(Assembly.Load).Select(GetAssemblyPath));
                }
            }

            var loadReferencingAssembly = this.razorConfiguration == null || this.razorConfiguration.AutoIncludeModelNamespace;

            if (loadReferencingAssembly && referencingAssembly != null)
            {
                assemblies.Add(GetAssemblyPath(referencingAssembly));
            }

            var compilerParameters = new CompilerParameters(assemblies.ToArray(), outputAssemblyName);
            var results = codeProvider.CompileAssemblyFromDom(compilerParameters, razorResult.GeneratedCode);

            if (results.Errors.HasErrors)
            {
                var err = results.Errors
                    .OfType<CompilerError>()
                    .Where(ce => !ce.IsWarning)
                    .First();

                var error = String.Format("Error Compiling Template: ({0}, {1}) {2})", err.Line, err.Column, err.ErrorText);

                return new NancyRazorErrorView(error);
            }
            // Load the assembly
            var assembly = Assembly.LoadFrom(outputAssemblyName);
            if (assembly == null)
            {
                const string error = "Error loading template assembly";
                return new NancyRazorErrorView(error);
            }

            // Get the template type
            var type = assembly.GetType("RazorOutput.RazorView");
            if (type == null) 
            {
                var error = String.Format("Could not find type RazorOutput.Template in assembly {0}", assembly.FullName);
                return new NancyRazorErrorView(error);
            }

            var view = Activator.CreateInstance(type) as NancyRazorViewBase;
            if (view == null)
            {
                const string error = "Could not construct RazorOutput.Template or it does not inherit from RazorViewBase";
                return new NancyRazorErrorView(error);
            }

            return view;
        }

        private static string GetAssemblyPath(Type type) {
            return GetAssemblyPath(type.Assembly);
        }

        private static string GetAssemblyPath(Assembly assembly) {
            return new Uri(assembly.CodeBase).LocalPath;
        }

        private NancyRazorViewBase GetOrCompileView(ViewLocationResult viewLocationResult, IRenderContext renderContext, Assembly referencingAssembly)
        {
            var view = renderContext.ViewCache.GetOrAdd(
                viewLocationResult, 
                x => GetCompiledView<dynamic>(x.Contents.Invoke(), referencingAssembly));

            view.Code = string.Empty;

            return view;
        }

        /// <summary>
        /// Gets the extensions file extensions that are supported by the view engine.
        /// </summary>
        /// <value>An <see cref="IEnumerable{T}"/> instance containing the extensions.</value>
        /// <remarks>The extensions should not have a leading dot in the name.</remarks>
        public IEnumerable<string> Extensions
        {
            get { return new[] { "cshtml" }; }
        }

        public void Initialize(ViewEngineStartupContext viewEngineStartupContext)
        {
            int a = 10;
        }

        /// <summary>
        /// Renders the view.
        /// </summary>
        /// <param name="viewLocationResult">A <see cref="ViewLocationResult"/> instance, containing information on how to get the view template.</param>
        /// <param name="model">The model that should be passed into the view</param>
        /// <param name="renderContext"></param>
        /// <returns>A delegate that can be invoked with the <see cref="Stream"/> that the view should be rendered to.</returns>
        public Action<Stream> RenderView(ViewLocationResult viewLocationResult, dynamic model, IRenderContext renderContext)
        {
            //@(section)?[\s]*(?<name>[A-Za-z]*)[\s]*{(?<content>[^\}]*)}?

            Assembly referencingAssembly = null;
            if (model != null)
            {
                var underlyingSystemType = model.GetType().UnderlyingSystemType;
                if (underlyingSystemType != null)
                {
                    referencingAssembly = Assembly.GetAssembly(underlyingSystemType);
                }
            }

            return stream =>
            {
                var view = 
                    GetOrCompileView(viewLocationResult, renderContext, referencingAssembly);

                var writer = 
                    new StreamWriter(stream);

                view.Html = new HtmlHelpers(this, renderContext);
                view.Model = model;
                view.Writer = writer;
                view.Execute();

                writer.Flush();
            };
        }
    }
}
