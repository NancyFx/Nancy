namespace Nancy.ViewEngines
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Reflection;

    /// <summary>
    /// Default implementation for extracting view information form an assembly.
    /// </summary>
    public class DefaultResourceReader : IResourceReader
    {
        /// <summary>
        /// Gets information about the resources that are embedded in the assembly.
        /// </summary>
        /// <param name="assembly">The <see cref="Assembly"/> to retrieve view information from.</param>
        /// <param name="supportedViewEngineExtensions">A list of view extensions to look for.</param>
        /// <returns>A <see cref="IList{T}"/> of resource locations and content readers.</returns>
        public IList<Tuple<string, Func<StreamReader>>> GetResourceStreamMatches(Assembly assembly, IEnumerable<string> supportedViewEngineExtensions)
        {
            var resourceStreams =
                from resourceName in assembly.GetManifestResourceNames()
                from viewEngineExtension in supportedViewEngineExtensions
                where GetResourceExtension(resourceName).Equals(viewEngineExtension, StringComparison.OrdinalIgnoreCase)
                select new Tuple<string, Func<StreamReader>>(
                    resourceName, 
                    () => new StreamReader(assembly.GetManifestResourceStream(resourceName)));

            return resourceStreams.ToList();
        }

        private static string GetResourceExtension(string resourceName)
        {
            return Path.GetExtension(resourceName).Substring(1);
        }
    }
}