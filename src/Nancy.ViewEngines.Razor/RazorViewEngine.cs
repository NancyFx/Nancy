namespace Nancy.ViewEngines.Razor
{
    using System;
    using System.CodeDom.Compiler;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Web.Razor;
    using Microsoft.CSharp;
    using System.Configuration;
    using System.Security.Policy;

    /// <summary>
    /// View engine for rendering razor views.
    /// </summary>
    public class RazorViewEngine : IViewEngine
    {
        private readonly RazorTemplateEngine engine;
        private readonly CodeDomProvider codeDomProvider;
        private readonly IRazorConfiguration razorConfiguration;

        /// <summary>
        /// Initializes a new instance of the <see cref="RazorViewEngine"/> class.
        /// </summary>
        ///  public RazorViewEngine(IRazorConfiguration configuration)
        public RazorViewEngine() : this(new RazorConfiguration(), null, new CSharpCodeProvider())
        {
        }

        public RazorViewEngine(IRazorConfiguration configuration)
            : this(configuration, null, new CSharpCodeProvider())
        {
        }

        public RazorViewEngine(IRazorConfiguration configuration, RazorTemplateEngine razorTemplateEngine, CodeDomProvider codeDomProvider)
        {
            this.codeDomProvider = codeDomProvider;
            this.razorConfiguration = configuration;
            this.engine = razorTemplateEngine ?? this.GetRazorTemplateEngine();
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
            host.NamespaceImports.Add("Microsoft.CSharp.RuntimeBinder");

            if (this.razorConfiguration != null)
            {
                foreach (var n in this.razorConfiguration.GetDefaultNamespaces())
                    host.NamespaceImports.Add(n);
            }

            return new RazorTemplateEngine(host);
        }

        private NancyRazorViewBase GetCompiledView<TModel>(TextReader reader, Assembly referencingAssembly) 
        {
            var razorResult = this.engine.GenerateCode(reader);

            string code;

            using (var sw = new StringWriter()) {
                this.codeDomProvider.GenerateCodeFromCompileUnit(razorResult.GeneratedCode, sw, new CodeGeneratorOptions());
                code = sw.GetStringBuilder().ToString();
            }

            var view =
                GenerateRazorView(this.codeDomProvider, razorResult, referencingAssembly);
            // TODO DEBUG ONLY

            view.Code = code;

            return view;
        }

        private NancyRazorViewBase GenerateRazorView(CodeDomProvider codeProvider, GeneratorResults razorResult, Assembly referencingAssembly)
        {
            // Compile the generated code into an assembly
            var outputAssemblyName =
                     Path.Combine(Path.GetTempPath(), String.Format("Temp_{0}.dll", Guid.NewGuid().ToString("N")));

            var assemblies = new List<string>
            {
                GetAssemblyPath(typeof(Microsoft.CSharp.RuntimeBinder.Binder))
                , GetAssemblyPath(typeof(System.Runtime.CompilerServices.CallSite))
                , GetAssemblyPath(Assembly.GetExecutingAssembly())
            };
            if (this.razorConfiguration != null)
            {
                foreach (var assemblyName in this.razorConfiguration.GetAssemblyNames())
                {
                    Assembly a = Assembly.Load(assemblyName);
                    assemblies.Add(GetAssemblyPath(a));
                }
            }

            var loadReferencingAssembly = this.razorConfiguration == null || this.razorConfiguration.AutoIncludeModelNamespace;

            if (loadReferencingAssembly)
                assemblies.Add(GetAssemblyPath(referencingAssembly));

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

        /// <summary>
        /// Gets the extensions file extensions that are supported by the view engine.
        /// </summary>
        /// <value>An <see cref="IEnumerable{T}"/> instance containing the extensions.</value>
        /// <remarks>The extensions should not have a leading dot in the name.</remarks>
        public IEnumerable<string> Extensions
        {
            get { return new[] { "cshtml", "vbhtml" }; }
        }

        /// <summary>
        /// Renders the view.
        /// </summary>
        /// <param name="viewLocationResult">A <see cref="ViewLocationResult"/> instance, containing information on how to get the view template.</param>
        /// <param name="model">The model that should be passed into the view</param>
        /// <returns>A delegate that can be invoked with the <see cref="Stream"/> that the view should be rendered to.</returns>
        public Action<Stream> RenderView(ViewLocationResult viewLocationResult, dynamic model)
        {
            Assembly referencingAssembly = null;
            if (model != null)
            {
                var underlyingSystemType = model.GetType().UnderlyingSystemType;
                if (underlyingSystemType != null)
                    referencingAssembly = Assembly.GetAssembly(underlyingSystemType);
            }
            return stream =>
            {
                var view =
                    GetCompiledView<dynamic>(viewLocationResult.Contents, referencingAssembly);

                var writer =
                    new StreamWriter(stream);

                view.Model = model;
                view.Writer = writer;
                view.Execute();
                writer.Flush();
            };
        }
    }
}