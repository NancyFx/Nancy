namespace Nancy.ViewEngines.Razor
{
    using System;
    using System.CodeDom;
    using System.CodeDom.Compiler;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Runtime.CompilerServices;
    using System.Text;
    using System.Web.Razor;

    using Microsoft.CSharp;

    using Nancy.Bootstrapper;
    using Nancy.Helpers;
    using Nancy.Responses;
    using Nancy.ViewEngines.Razor.CSharp;
    using Nancy.ViewEngines.Razor.VisualBasic;

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
        public RazorViewEngine(IRazorConfiguration configuration)
        {
            this.viewRenderers = new List<IRazorViewRenderer>
            {
                new CSharpRazorViewRenderer(),
                new VisualBasicRazorViewRenderer()
            };

            this.razorConfiguration = configuration;

            foreach (var renderer in this.viewRenderers)
            {
                this.AddDefaultNameSpaces(renderer.Host);
            }
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
            return RenderView(viewLocationResult, model, renderContext, false);
        }

        /// <summary>
        /// Renders the view.
        /// </summary>
        /// <param name="viewLocationResult">A <see cref="ViewLocationResult"/> instance, containing information on how to get the view template.</param>
        /// <param name="model">The model that should be passed into the view</param>
        /// <param name="renderContext">The render context.</param>
        /// <param name="isPartial">Used by HtmlHelpers to declare a view as partial</param>
        /// <returns>A response.</returns>
        public Response RenderView(ViewLocationResult viewLocationResult, dynamic model, IRenderContext renderContext, bool isPartial)
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
                var writer =
                    new StreamWriter(stream);

                var view = this.GetViewInstance(viewLocationResult, renderContext, referencingAssembly, model);

                view.ExecuteView(null, null);

                var body = view.Body;
                var sectionContents = view.SectionContents;

                var layout = view.HasLayout ? view.Layout : GetViewStartLayout(model, renderContext, referencingAssembly, isPartial);

                var root = string.IsNullOrWhiteSpace(layout);

                while (!root)
                {
                    var viewLocation =
                        renderContext.LocateView(layout, model);

                    if (viewLocation == null)
                    {
                        throw new InvalidOperationException("Unable to locate layout: " + layout);
                    }

                    view = this.GetViewInstance(viewLocation, renderContext, referencingAssembly, model);

                    view.ExecuteView(body, sectionContents);

                    body = view.Body;
                    sectionContents = view.SectionContents;

                    layout = view.HasLayout ? view.Layout : GetViewStartLayout(model, renderContext, referencingAssembly, isPartial);

                    root = !view.HasLayout;
                }

                writer.Write(body);
                writer.Flush();
            };

            return response;
        }

        private string GetViewStartLayout(dynamic model, IRenderContext renderContext, Assembly referencingAssembly, bool isPartial)
        {
            if (isPartial)
            {
                return string.Empty;
            }

            var view = renderContext.LocateView("_ViewStart", model);

            if (view == null)
            {
                return string.Empty;
            }

            if (!this.Extensions.Any(x => x.Equals(view.Extension, StringComparison.OrdinalIgnoreCase)))
            {
                return string.Empty;
            }

            var viewInstance = GetViewInstance(view, renderContext, referencingAssembly, model);

            viewInstance.ExecuteView(null, null);

            return viewInstance.Layout ?? string.Empty;
        }

        private void AddDefaultNameSpaces(RazorEngineHost engineHost)
        {
            engineHost.NamespaceImports.Add("System");
            engineHost.NamespaceImports.Add("System.IO");

            if (this.razorConfiguration != null)
            {
                var namespaces = this.razorConfiguration.GetDefaultNamespaces();

                if (namespaces == null)
                {
                    return;
                }

                foreach (var n in namespaces.Where(n => !string.IsNullOrWhiteSpace(n)))
                {
                    engineHost.NamespaceImports.Add(n);
                }
            }
        }

        private Func<INancyRazorView> GetCompiledViewFactory(string extension, TextReader reader, Assembly referencingAssembly, Type passedModelType, ViewLocationResult viewLocationResult)
        {
            var renderer = this.viewRenderers.First(x => x.Extension.Equals(extension, StringComparison.OrdinalIgnoreCase));

            var engine = new RazorTemplateEngine(renderer.Host);

            var razorResult = engine.GenerateCode(reader, null, null, "roo");

            var viewFactory = this.GenerateRazorViewFactory(renderer, razorResult, referencingAssembly, passedModelType, viewLocationResult);

            return viewFactory;
        }

        private Func<INancyRazorView> GenerateRazorViewFactory(IRazorViewRenderer viewRenderer, GeneratorResults razorResult, Assembly referencingAssembly, Type passedModelType, ViewLocationResult viewLocationResult)
        {
            var outputAssemblyName =
                Path.Combine(Path.GetTempPath(), String.Format("Temp_{0}.dll", Guid.NewGuid().ToString("N")));

            var modelType = (Type)razorResult.GeneratedCode.Namespaces[0].Types[0].UserData["ModelType"]
                            ?? passedModelType
                            ?? typeof(object);

            var assemblies = new List<string>
            {
                GetAssemblyPath(typeof(CallSite)),
                GetAssemblyPath(typeof(IHtmlString)),
                GetAssemblyPath(Assembly.GetExecutingAssembly()),
                GetAssemblyPath(modelType)
            };

            assemblies.AddRange(AppDomainAssemblyTypeScanner.Assemblies.Select(GetAssemblyPath));

            if (referencingAssembly != null)
            {
                assemblies.Add(GetAssemblyPath(referencingAssembly));
            }

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

            assemblies = assemblies
                .Union(viewRenderer.Assemblies)
                .ToList();

            var compilerParameters =
                new CompilerParameters(assemblies.ToArray(), outputAssemblyName);

            CompilerResults results;
            lock (this.compileLock)
            {
                results = viewRenderer.Provider.CompileAssemblyFromDom(compilerParameters, razorResult.GeneratedCode);
            }

            if (results.Errors.HasErrors)
            {
                var output = new string[results.Output.Count];
                results.Output.CopyTo(output, 0);

                var fullTemplateName = viewLocationResult.Location + "/" + viewLocationResult.Name + "." + viewLocationResult.Extension;
                var templateLines = GetViewBodyLines(viewLocationResult);
                var errors = results.Errors.OfType<CompilerError>().Where(ce => !ce.IsWarning).ToArray();
                var errorMessages = BuildErrorMessages(errors);
                var compilationSource = this.GetCompilationSource(viewRenderer.Provider, razorResult.GeneratedCode);

                MarkErrorLines(errors, templateLines);

                var lineNumber = 1;

                var errorDetails = string.Format(
                                        "Error compiling template: <strong>{0}</strong><br/><br/>Errors:<br/>{1}<br/><br/>Details:<br/>{2}<br/><br/>Compilation Source:<br/><pre><code>{3}</code></pre>",
                                        fullTemplateName,
                                        errorMessages,
                                        templateLines.Aggregate((s1, s2) => s1 + "<br/>" + s2),
                                        compilationSource.Aggregate((s1, s2) => s1 + "<br/>Line " + lineNumber++ + ":\t" + s2));

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

        private string[] GetCompilationSource(CodeDomProvider provider, CodeCompileUnit generatedCode)
        {
            var compilationSourceBuilder = new StringBuilder();
            using (var writer = new IndentedTextWriter(new StringWriter(compilationSourceBuilder), "\t"))
            {
                provider.GenerateCodeFromCompileUnit(generatedCode, writer, new CodeGeneratorOptions());
            }

            var compilationSource = compilationSourceBuilder.ToString();
            return HttpUtility.HtmlEncode(compilationSource)
                .Split(new[] { Environment.NewLine }, StringSplitOptions.None);
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
                if ((lineIndex <= templateLines.Count - 1) && (lineIndex >= 0))
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
                    templateLines.Add(HttpUtility.HtmlEncode(currentLine));

                    currentLine = templateReader.ReadLine();
                }
            }
            return templateLines.ToArray();
        }

        private static void AddModelNamespace(GeneratorResults razorResult, Type modelType)
        {
            if (string.IsNullOrWhiteSpace(modelType.Namespace))
            {
                return;
            }

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
                x =>
                {
                    using (var reader = x.Contents.Invoke())
                        return this.GetCompiledViewFactory(x.Extension, reader, referencingAssembly, passedModelType, viewLocationResult);
                });

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