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

    public class RazorViewCompiler : IViewCompiler, IViewEngine
    {
        private readonly RazorTemplateEngine engine;
        private readonly CodeDomProvider codeDomProvider;

        public RazorViewCompiler()
            : this(GetRazorTemplateEngine(), new CSharpCodeProvider())
        {
        }

        public RazorViewCompiler(RazorTemplateEngine razorTemplateEngine, CodeDomProvider codeDomProvider)
        {
            this.engine = razorTemplateEngine;
            this.codeDomProvider = codeDomProvider;
        }

        private static RazorTemplateEngine GetRazorTemplateEngine()
        {
            var host = 
                new RazorEngineHost(new CSharpRazorCodeLanguage())
                {
                    DefaultBaseClass = typeof(RazorViewBase).FullName,
                    DefaultNamespace = "RazorOutput",
                    DefaultClassName = "RazorView"
                };

            host.NamespaceImports.Add("System");
            host.NamespaceImports.Add("System.IO");
            host.NamespaceImports.Add("Microsoft.CSharp.RuntimeBinder");

            return new RazorTemplateEngine(host);
        }

        public IView GetCompiledView<TModel>(TextReader reader) 
        {
            var razorResult = this.engine.GenerateCode(reader);

            string code;

            using (var sw = new StringWriter()) {
                this.codeDomProvider.GenerateCodeFromCompileUnit(razorResult.GeneratedCode, sw, new CodeGeneratorOptions());
                code = sw.GetStringBuilder().ToString();
            }

            var view = 
                GenerateRazorView(this.codeDomProvider, razorResult);
            // TODO DEBUG ONLY
            view.Code = code;

            return view;
        }

        private static RazorViewBase GenerateRazorView(CodeDomProvider codeProvider, GeneratorResults razorResult)
        {
            // Compile the generated code into an assembly

            var outputAssemblyName =
                Path.Combine(Path.GetTempPath(), String.Format("Temp_{0}.dll", Guid.NewGuid().ToString("N")));

            var results = codeProvider.CompileAssemblyFromDom(
                new CompilerParameters(new [] {
                    GetAssemblyPath(typeof(IView)),
                    GetAssemblyPath(typeof(Microsoft.CSharp.RuntimeBinder.Binder)), 
                    GetAssemblyPath(typeof(System.Runtime.CompilerServices.CallSite)), 
                    GetAssemblyPath(Assembly.GetExecutingAssembly())}, outputAssemblyName),
                    razorResult.GeneratedCode);

            if (results.Errors.HasErrors)
            {
                var err = results.Errors
                    .OfType<CompilerError>()
                    .Where(ce => !ce.IsWarning)
                    .First();

                var error = String.Format("Error Compiling Template: ({0}, {1}) {2})", err.Line, err.Column, err.ErrorText);

                return new ErrorView(error);
            }
            // Load the assembly
            var assembly = Assembly.LoadFrom(outputAssemblyName);
            if (assembly == null)
            {
                const string error = "Error loading template assembly";
                return new ErrorView(error);
            }

            // Get the template type
            var type = assembly.GetType("RazorOutput.RazorView");
            if (type == null) 
            {
                var error = String.Format("Could not find type RazorOutput.Template in assembly {0}", assembly.FullName);
                return new ErrorView(error);
            }

            var view = Activator.CreateInstance(type) as RazorViewBase;
            if (view == null)
            {
                const string error = "Could not construct RazorOutput.Template or it does not inherit from RazorViewBase";
                return new ErrorView(error);
            }

            return view;
        }

        private static string GetAssemblyPath(Type type) {
            return GetAssemblyPath(type.Assembly);
        }

        private static string GetAssemblyPath(Assembly assembly) {
            return new Uri(assembly.CodeBase).LocalPath;
        }

        public IEnumerable<string> Extensions
        {
            get { return new[] { "cshtml", "vbhtml" }; }
        }

        public Action<Stream> RenderView(ViewLocationResult viewLocationResult, dynamic model)
        {
            return stream =>
            {
                var view = 
                    GetCompiledView<dynamic>(viewLocationResult.Contents);

                using (var writer = new StreamWriter(stream))
                {
                    view.Model = model;
                    view.Writer = writer;
                    view.Execute();
                }
            };
        }
    }
}