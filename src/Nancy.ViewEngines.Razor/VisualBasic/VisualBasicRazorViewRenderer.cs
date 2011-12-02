namespace Nancy.ViewEngines.Razor.VisualBasic
{
    using System;
    using System.CodeDom.Compiler;
    using System.Collections.Generic;
    using System.Web.Razor;
    using Microsoft.VisualBasic;

    /// <summary>
    /// Renderer for Visual Basic razor files.
    /// </summary>
    public class VisualBasicRazorViewRenderer : IRazorViewRenderer
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
            get { return "vbhtml"; }
        }

        /// <summary>
        /// Gets the host.
        /// </summary>
        public RazorEngineHost Host { get; private set; }

        /// <summary>
        /// Gets the provider that is used to generate code.
        /// </summary>
        public CodeDomProvider Provider { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="VisualBasicRazorViewRenderer"/> class.
        /// </summary>
        public VisualBasicRazorViewRenderer()
        {
            this.Assemblies = new List<string>();

            this.Provider = new VBCodeProvider();

            this.Host = new NancyRazorEngineHost(new VBRazorCodeLanguage());
        }

    }
}
