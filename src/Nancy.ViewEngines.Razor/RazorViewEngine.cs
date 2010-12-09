using System;
using System.CodeDom.Compiler;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web.Hosting;
using System.Web.Razor;
using Microsoft.CSharp;

namespace Nancy.ViewEngines.Razor {
    public class RazorViewEngine {
        private static RazorTemplateEngine _engine = RazorViewEngine.GetRazorTemplateEngine();

        private static RazorTemplateEngine GetRazorTemplateEngine() {
            RazorEngineHost host = new RazorEngineHost(new CSharpRazorCodeLanguage());

            host.DefaultBaseClass = typeof(RazorViewBase).FullName;

            host.DefaultNamespace = "RazorOutput";
            host.DefaultClassName = "RazorView";

            host.NamespaceImports.Add("System");
            host.NamespaceImports.Add("System.IO");
            host.NamespaceImports.Add("Microsoft.CSharp.RuntimeBinder");

            return new RazorTemplateEngine(host);
        }

        public void RenderView(string name, Stream stream, dynamic model) {
            string path = HostingEnvironment.MapPath(name);

            var view = GenerateRazorView(path);
            view.Model = model;
            using (var writer = new StreamWriter(stream)) {
                view.Writer = writer;
                view.Execute();
                writer.Flush();
            }
        }

        private RazorViewBase GenerateRazorView(string templatePath) {
            GeneratorResults razorResult = null;
            using (TextReader reader = new StreamReader(templatePath)) {
                razorResult = _engine.GenerateCode(reader);
            }

            CSharpCodeProvider codeProvider = new CSharpCodeProvider();

            string code = null;
            using (StringWriter sw = new StringWriter()) {
                codeProvider.GenerateCodeFromCompileUnit(razorResult.GeneratedCode, sw, new CodeGeneratorOptions());
                code = sw.GetStringBuilder().ToString();
            }

            var view = GenerateRazorView(codeProvider, razorResult);
            // TODO DEBUG ONLY
            view.Code = code;
            return view;
        }

        private string GetAssemblyPath(Type type) {
            return GetAssemblyPath(type.Assembly);
        }

        private string GetAssemblyPath(Assembly assembly) {
            return new Uri(assembly.CodeBase).LocalPath;
        }

        private RazorViewBase GenerateRazorView(CodeDomProvider codeProvider, GeneratorResults razorResult) {
            // Compile the generated code into an assembly
            string outputAssemblyName = String.Format("Temp_{0}.dll", Guid.NewGuid().ToString("N"));

            CompilerResults results = codeProvider.CompileAssemblyFromDom(

                new CompilerParameters(new string[] { 
                    GetAssemblyPath(typeof(Microsoft.CSharp.RuntimeBinder.Binder)), 
                    GetAssemblyPath(typeof(System.Runtime.CompilerServices.CallSite)), 
                    GetAssemblyPath(Assembly.GetExecutingAssembly()) }, outputAssemblyName),
                razorResult.GeneratedCode);

            if (results.Errors.HasErrors) {
                CompilerError err = results.Errors
                                           .OfType<CompilerError>()
                                           .Where(ce => !ce.IsWarning)
                                           .First();
                var error = String.Format("Error Compiling Template: ({0}, {1}) {2})",
                                              err.Line, err.Column, err.ErrorText);

                return new ErrorView(error);
            }
            else {
                // Load the assembly
                Assembly assembly = Assembly.LoadFrom(outputAssemblyName);
                if (assembly == null) {
                    string error = "Error loading template assembly";

                    return new ErrorView(error);
                }
                else {
                    // Get the template type
                    Type type = assembly.GetType("RazorOutput.RazorView");
                    if (type == null) {
                        string error = String.Format("Could not find type RazorOutput.Template in assembly {0}", assembly.FullName);
                        return new ErrorView(error);
                    }
                    else {
                        RazorViewBase view = Activator.CreateInstance(type) as RazorViewBase;
                        if (view == null) {
                            string error = "Could not construct RazorOutput.Template or it does not inherit from RazorViewBase";
                            return new ErrorView(error);
                        }
                        else {
                            return view;
                        }
                    }
                }
            }
        }

    }
}
