namespace Nancy.ViewEngines.Razor
{
    using System;
    using System.Linq;

    internal static class CodeParserHelper
    {
        /// <summary>
        /// Throws exception says that given type was not found in any accessible assembly
        /// </summary>
        /// <param name="razorAssemblyProvider">An <see cref="RazorAssemblyProvider"/> instance.</param>
        /// <param name="type">Type that was not found</param>
        public static void ThrowTypeNotFound(RazorAssemblyProvider razorAssemblyProvider, string type)
        {
            throw new NotSupportedException(string.Format(
                "Unable to discover CLR Type for model by the name of {0}.\n\nTry using a fully qualified type name and ensure that the assembly is added to the configuration file.\n\nCurrent RazorAssemblyProvider assemblies:\n\t{1}.",
                type,
                razorAssemblyProvider.GetAssemblies().Select(a => a.FullName).Aggregate((n1, n2) => n1 + "\n\t" + n2)));
        }
    }
}
