namespace Nancy.ViewEngines.Razor
{
    using System;
    using System.CodeDom;
    using System.CodeDom.Compiler;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Web.Razor;
    using System.Web.Razor.Parser.SyntaxTree;
    using Nancy.Bootstrapper;
    using Nancy.Responses;
    using Nancy.Localization;
    using Nancy.ViewEngines.Razor.CSharp;

    /// <summary>
    /// View engine for rendering razor views.
    /// </summary>
    public class RazorViewEngine : IViewEngine, IDisposable
    {
        private readonly IRazorConfiguration razorConfiguration;
        private readonly IEnumerable<IRazorViewRenderer> viewRenderers;
        private readonly object compileLock = new object();

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
        /// Initializes a new instance of the <see cref="RazorViewEngine"/> class.
        /// </summary>
        /// <param name="configuration">The <see cref="IRazorConfiguration"/> that should be used by the engine.</param>
        /// <param name="textResource">The <see cref="ITextResource"/> that should be used by the engine.</param>
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

        private Func<INancyRazorView> GetCompiledViewFactory(string extension, TextReader reader, Assembly referencingAssembly, Type passedModelType, ViewLocationResult viewLocationResult)
        {
            var renderer = this.viewRenderers.First(x => x.Extension.Equals(extension, StringComparison.OrdinalIgnoreCase));

            var engine = this.GetRazorTemplateEngine(renderer.Host);

            var razorResult = engine.GenerateCode(reader, null, null, "roo");

            var viewFactory = this.GenerateRazorViewFactory(renderer.Provider, razorResult, referencingAssembly, renderer.Assemblies, passedModelType, viewLocationResult);

            return viewFactory;
        }

        private Func<INancyRazorView> GenerateRazorViewFactory(CodeDomProvider codeProvider, GeneratorResults razorResult, Assembly referencingAssembly, IEnumerable<string> rendererSpecificAssemblies, Type passedModelType, ViewLocationResult viewLocationResult)
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

            CompilerResults results;
            lock (this.compileLock)
            {
                results = codeProvider.CompileAssemblyFromDom(compilerParameters, razorResult.GeneratedCode);
            }

            if (results.Errors.HasErrors)
            {
                var output = new string[results.Output.Count];
                results.Output.CopyTo(output, 0);
                var outputString = string.Join("\n", output);

                var fullTemplateName = viewLocationResult.Location + "/" + viewLocationResult.Name + "." + viewLocationResult.Extension;
                var templateLines = GetViewBodyLines(viewLocationResult);
                var errors = results.Errors.OfType<CompilerError>().Where(ce => !ce.IsWarning).ToArray();
                var errorMessages = BuildErrorMessages(errors);

                MarkErrorLines(errors, templateLines);

                var errorDetails = string.Format(
                                        "Error compiling template: <strong>{0}</strong><br/><br/>Errors:<br/>{1}<br/><br/>Details:<br/>{2}",
                                        fullTemplateName,
                                        errorMessages,
                                        templateLines.Aggregate((s1, s2) => s1 + "<br/>" + s2));

                return () => new NancyRazorErrorView(errorDetails);
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

            if (Activator.CreateInstance(type) as INancyRazorView == null)
            {
                const string error = "Could not construct RazorOutput.Template or it does not inherit from INancyRazorView";
                return () => new NancyRazorErrorView(error);
            }

            return () => (INancyRazorView)Activator.CreateInstance(type);
        }

        private static string BuildErrorMessages(IEnumerable<CompilerError> errors)
        {
            return errors.Select(error => String.Format(
                "[{0}] Line: {1} Column: {2} - {3} (<a class='LineLink' href='#{1}'>show</a>)",
                error.ErrorNumber,
                error.Line,
                error.Column,
                error.ErrorText)).Aggregate((s1, s2) => s1 + "<br/>" + s2);
        }

        private static void MarkErrorLines(IEnumerable<CompilerError> errors, IList<string> templateLines)
        {
            foreach (var compilerError in errors)
            {
                var lineIndex = compilerError.Line - 1;
                if (lineIndex <= templateLines.Count - 1)
                {
                    templateLines[lineIndex] = string.Format("<span class='error'><a name='{0}' />{1}</span>", compilerError.Line, templateLines[lineIndex]);
                }
            }
        }

        private static string[] GetViewBodyLines(ViewLocationResult viewLocationResult)
        {
            var templateLines = new List<string>();
            using (var templateReader = viewLocationResult.Contents.Invoke())
            {
                var currentLine = templateReader.ReadLine();
                while (currentLine != null)
                {
                    templateLines.Add(Helpers.HttpUtility.HtmlEncode(currentLine));

                    currentLine = templateReader.ReadLine();
                }
            }
            return templateLines.ToArray();
        }

        /// <summary>
        /// Tries to find the model type from the document
        /// So documents using @model will actually be able to reference the model type
        /// </summary>
        /// <param name="block">The document</param>
        /// <param name="passedModelType">The model type from the base class</param>
        /// <returns>The model type, if discovered, or the passedModelType if not</returns>
        private static Type FindModelType(Block block, Type passedModelType)
        {
            var modelBlock =
                block.Flatten().FirstOrDefault(b => b.CodeGenerator.GetType() == typeof(ModelCodeGenerator));

            if (modelBlock == null)
            {
                return passedModelType ?? typeof(object);
            }

            if (string.IsNullOrEmpty(modelBlock.Content))
            {
                return passedModelType ?? typeof(object);
            }

            var discoveredModelType = modelBlock.Content.Trim();

            var modelType = Type.GetType(discoveredModelType);

            if (modelType != null)
            {
                return modelType;
            }

            modelType = AppDomainAssemblyTypeScanner.Types.FirstOrDefault(t => t.FullName == discoveredModelType);

            if (modelType != null)
            {
                return modelType;
            }

            modelType = AppDomainAssemblyTypeScanner.Types.FirstOrDefault(t => t.Name == discoveredModelType);

            if (modelType != null)
            {
                return modelType;
            }

            throw new NotSupportedException(string.Format("Unable to discover CLR Type for model by the name of {0}. Try using a fully qualified type name and ensure that the assembly is added to the configuration file.", discoveredModelType));
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

        private INancyRazorView GetOrCompileView(ViewLocationResult viewLocationResult, IRenderContext renderContext, Assembly referencingAssembly, Type passedModelType)
        {
            var viewFactory = renderContext.ViewCache.GetOrAdd(
                viewLocationResult,
                x => this.GetCompiledViewFactory(x.Extension, x.Contents.Invoke(), referencingAssembly, passedModelType, viewLocationResult));

            var view = viewFactory.Invoke();

            return view;
        }

        private INancyRazorView GetViewInstance(ViewLocationResult viewLocationResult, IRenderContext renderContext, Assembly referencingAssembly, dynamic model)
        {
            var modelType = (model == null) ? typeof(object) : model.GetType();

            var view =
                this.GetOrCompileView(viewLocationResult, renderContext, referencingAssembly, modelType);

            view.Initialize(this, renderContext, model);

            return view;
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <filterpriority>2</filterpriority>
        public void Dispose()
        {
            if (this.viewRenderers == null)
            {
                return;
            }

            foreach (var disposable in this.viewRenderers.OfType<IDisposable>())
            {
                disposable.Dispose();
            }
        }
    }
}