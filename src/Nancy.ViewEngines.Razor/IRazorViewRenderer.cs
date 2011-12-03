namespace Nancy.ViewEngines.Razor
{
    using System.Collections.Generic;
    using System.CodeDom.Compiler;
    using System.Web.Razor;

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
        /// Gets the extension this view renderer supports.
        /// </summary>
        string Extension { get; }

        /// <summary>
        /// Gets the host.
        /// </summary>
        RazorEngineHost Host { get; }

        /// <summary>
        /// Gets the provider that is used to generate code.
        /// </summary>
        CodeDomProvider Provider { get; }
    }
}
