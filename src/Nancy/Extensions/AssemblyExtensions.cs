namespace Nancy.Extensions
{
    using System;
    using System.IO;
    using System.Reflection;

    /// <summary>
    /// Assembly extension methods
    /// </summary>
    public static class AssemblyExtensions
    {
        /// <summary>
        /// Gets exported types from an assembly and catches common errors
        /// that occur when running under test runners.
        /// </summary>
        /// <param name="assembly">Assembly to retrieve from</param>
        /// <returns>An array of types</returns>
        public static Type[] SafeGetExportedTypes(this Assembly assembly)
        {
            Type[] types;

            try
            {
                types = assembly.GetExportedTypes();
            }
            catch (FileNotFoundException)
            {
                types = ArrayCache.Empty<Type>();
            }
            catch (NotSupportedException)
            {
                types = ArrayCache.Empty<Type>();
            }
            catch (FileLoadException) {
                // probably assembly version conflict
                types = ArrayCache.Empty<Type>();
            }
            return types;
        }

#if !CORE
        /// <summary>
        /// Indicates if a given assembly references another which is identified by its name.
        /// </summary>
        /// <param name="assembly">The assembly which will be probed.</param>
        /// <param name="referenceName">The reference assembly name.</param>
        /// <returns>A boolean value indicating if there is a reference.</returns>
        public static bool IsReferencing(this Assembly assembly, AssemblyName referenceName)
        {
            if (AssemblyName.ReferenceMatchesDefinition(assembly.GetName(), referenceName))
            {
                return true;
            }

            foreach (var referencedAssemblyName in assembly.GetReferencedAssemblies())
            {
                if (AssemblyName.ReferenceMatchesDefinition(referencedAssemblyName, referenceName))
                {
                    return true;
                }
            }

            return false;
        }
#endif
    }
}
