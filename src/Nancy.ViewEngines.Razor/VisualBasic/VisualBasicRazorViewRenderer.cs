namespace Nancy.ViewEngines.Razor.VisualBasic
{
    using System;
    using System.CodeDom.Compiler;
    using System.Collections.Generic;
    using System.Web.Razor;
    using System.Web.Razor.Generator;
    using Microsoft.VisualBasic;

    /// <summary>
    /// Renderer for Visual Basic razor files.
    /// </summary>
    public class VisualBasicRazorViewRenderer : IRazorViewRenderer, IDisposable
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
            get { return "vbhtml"; }
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
        /// Initializes a new instance of the <see cref="VisualBasicRazorViewRenderer"/> class.
        /// </summary>
        public VisualBasicRazorViewRenderer(IRazorEngineHostFactory engineHostFactory)
        {
            this.engineHostFactory = engineHostFactory;
            this.ModelCodeGenerator = typeof(VisualBasicModelCodeGenerator);

            this.Assemblies = new List<string>();

            this.Provider = new VBCodeProvider();
        }

        /// <summary>
        /// Get the host
        /// </summary>
        public NancyRazorEngineHost GetHost(string viewLocation)
        {
            return this.engineHostFactory.Create(new VBRazorCodeLanguage(), viewLocation);
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
