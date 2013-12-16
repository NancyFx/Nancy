namespace Nancy.ViewEngines.Razor.CSharp
{
    using System;
    using System.CodeDom.Compiler;
    using System.Collections.Generic;
    using System.Web.Razor;
    using System.Web.Razor.Generator;
    using Microsoft.CSharp;

    using Nancy.Extensions;

    /// <summary>
    /// Renderer for CSharp razor files.
    /// </summary>
    public class CSharpRazorViewRenderer : IRazorViewRenderer, IDisposable
    {
        private readonly IRazorEngineHostFactory engineHostFactory;

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
        /// Gets the provider that is used to generate code.
        /// </summary>
        public CodeDomProvider Provider { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="CSharpRazorViewRenderer"/> class.
        /// </summary>
        public CSharpRazorViewRenderer(IRazorEngineHostFactory engineHostFactory)
        {
            this.engineHostFactory = engineHostFactory;
            this.Assemblies = new List<string>
            {
                typeof(Microsoft.CSharp.RuntimeBinder.Binder).GetAssemblyPath()
            };

            this.ModelCodeGenerator = typeof(CSharpModelCodeGenerator);

            this.Provider = new CSharpCodeProvider();
        }

        /// <summary>
        /// Get the host
        /// </summary>
        public NancyRazorEngineHost GetHost(string viewLocation)
        {
            var host = this.engineHostFactory.Create(new CSharpRazorCodeLanguage(), viewLocation);

            host.NamespaceImports.Add("Microsoft.CSharp.RuntimeBinder");

            return host;
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <filterpriority>2</filterpriority>
        public void Dispose()
        {
            if (this.Provider != null)
            {
                this.Provider.Dispose();
            }
        }
    }
}