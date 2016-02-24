namespace Nancy.ViewEngines.Razor.CSharp
{
    using System;
    using System.CodeDom.Compiler;
    using System.Collections.Generic;
    using System.Web.Razor;
    using Microsoft.CSharp.RuntimeBinder;

    /// <summary>
    /// Renderer for CSharp razor files.
    /// </summary>
    public class CSharpRazorViewRenderer : IRazorViewRenderer, IDisposable
    {
        /// <summary>
        /// Gets the assemblies.
        /// </summary>
        public IEnumerable<string> Assemblies { get; private set; }

        /// <summary>
        /// Gets the extension this view renderer supports.
        /// </summary>
        public string Extension
        {
            get { return "cshtml"; }
        }

        /// <summary>
        /// Gets the <see cref="SetBaseTypeCodeGenerator"/> that should be used with the renderer.
        /// </summary>
        public Type ModelCodeGenerator { get; private set; }

        /// <summary>
        /// Gets the host.
        /// </summary>
        public RazorEngineHost Host { get; private set; }

        /// <summary>
        /// Gets the provider that is used to generate code.
        /// </summary>
        public CodeDomProvider Provider { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="CSharpRazorViewRenderer"/> class.
        /// </summary>
        public CSharpRazorViewRenderer(RazorAssemblyProvider razorAssemblyProvider)
        {
            this.Assemblies = new List<string>
            {
                typeof(Binder).Assembly.Location
            };

            this.ModelCodeGenerator = typeof(ModelCodeGenerator);

            this.Provider = new Microsoft.CodeDom.Providers.DotNetCompilerPlatform.CSharpCodeProvider();

            this.Host = new NancyRazorEngineHost(new CSharpRazorCodeLanguage(), razorAssemblyProvider);

            this.Host.NamespaceImports.Add("Microsoft.CSharp.RuntimeBinder");
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            if (this.Provider != null)
            {
                this.Provider.Dispose();
            }
        }
    }
}
