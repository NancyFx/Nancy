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
    }
}
