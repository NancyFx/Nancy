namespace Nancy.ViewEngines.Razor.CSharp
{
    using System.CodeDom.Compiler;
    using System.Collections.Generic;
    using System.Web.Razor;

    using Microsoft.CSharp;

    using Nancy.Extensions;

    /// <summary>
    /// Renderer for CSharp razor files.
    /// </summary>
    public class CSharpRazorViewRenderer : IRazorViewRenderer
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
        /// Gets the host.
        /// </summary>
        public RazorEngineHost Host { get; private set; }

        /// <summary>
        /// Creates the provider that is used to generate code.
        /// </summary>
        public CodeDomProvider CreateProvider()
        {
            return new CSharpCodeProvider();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CSharpRazorViewRenderer"/> class.
        /// </summary>
        public CSharpRazorViewRenderer()
        {
            this.Assemblies = new List<string>
            {
                typeof(Microsoft.CSharp.RuntimeBinder.Binder).GetAssemblyPath()
            };

            this.Host = new NancyRazorEngineHost(new CSharpRazorCodeLanguage());

            this.Host.NamespaceImports.Add("Microsoft.CSharp.RuntimeBinder");
        }
    }
}