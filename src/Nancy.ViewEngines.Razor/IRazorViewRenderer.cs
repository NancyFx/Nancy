namespace Nancy.ViewEngines.Razor
{
    using System;
    using System.CodeDom.Compiler;
    using System.Collections.Generic;
    using System.Web.Razor.Generator;

    /// <summary>
    /// Renders a view.
    /// </summary>
    public interface IRazorViewRenderer
    {
        /// <summary>
        /// Gets the assemblies.
        /// </summary>
        IEnumerable<string> Assemblies { get; }

        /// <summary>
        /// Gets the <see cref="SetBaseTypeCodeGenerator"/> that should be used with the renderer.
        /// </summary>
        Type ModelCodeGenerator { get; }

        /// <summary>
        /// Gets the extension this view renderer supports.
        /// </summary>
        string Extension { get; }

        /// <summary>
        /// Gets the host.
        /// </summary>
        NancyRazorEngineHost GetHost(string viewLocation);

        /// <summary>
        /// Gets the provider that is used to generate code.
        /// </summary>
        CodeDomProvider Provider { get; }
    }
}
