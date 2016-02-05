namespace Nancy.ViewEngines.Razor
{
    using System;
    using System.Linq;

    internal static class CodeParserHelper
    {
        /// <summary>
        /// Throws exception says that given type was not found in any accessible assembly
        /// </summary>
        /// <param name="assemblyCatalog">An <see cref="IAssemblyCatalog"/> instance.</param>
        /// <param name="type">Type that was not found</param>
        public static void ThrowTypeNotFound(IAssemblyCatalog assemblyCatalog, string type)
        {
            throw new NotSupportedException(string.Format(
                "Unable to discover CLR Type for model by the name of {0}.\n\nTry using a fully qualified type name and ensure that the assembly is added to the configuration file.\n\nCurrent IAssemblyCatalog assemblies:\n\t{1}.",
                type,
                assemblyCatalog.GetAssemblies().Select(a => a.FullName).Aggregate((n1, n2) => n1 + "\n\t" + n2)));
        }
    }
}
