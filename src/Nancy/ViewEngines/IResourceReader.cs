namespace Nancy.ViewEngines
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Reflection;

    /// <summary>
    /// Defines the functionality of a reader that extracts embedded views from an assembly.
    /// </summary>
    public interface IResourceReader
    {
        /// <summary>
        /// Gets information about the resources that are embedded in the assembly.
        /// </summary>
        /// <param name="assembly">The <see cref="Assembly"/> to retrieve view information from.</param>
        /// <param name="supportedViewEngineExtensions">A list of view extensions to look for.</param>
        /// <returns>A <see cref="IList{T}"/> of resource locations and content readers.</returns>
        IList<Tuple<string, Func<StreamReader>>> GetResourceStreamMatches(Assembly assembly, IEnumerable<string> supportedViewEngineExtensions);
    }
}