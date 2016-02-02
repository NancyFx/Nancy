namespace Nancy
{
    using System;
    using System.Linq;

    /// <summary>
    /// Default <see cref="AssemblyResolveStrategy"/> implementations.
    /// </summary>
    public class AssemblyResolveStrategies
    {
        /// <summary>
        /// Resolve all available assemblies.
        /// </summary>
        public static readonly AssemblyResolveStrategy All = assembly =>
        {
            return true;
        };

        /// <summary>
        /// Resolves all assemblies that references Nancy.
        /// </summary>
        public static readonly AssemblyResolveStrategy NancyReferencing = assembly =>
        {
            if (assembly.Equals(typeof(INancyEngine).Assembly))
            {
                return true;
            }

            if (assembly.GetName().Name.Equals("Nancy.Testing", StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }

            if (assembly.GetReferencedAssemblies().Any(reference => reference.Name.StartsWith("Nancy", StringComparison.OrdinalIgnoreCase)))
            {
                return true;
            }

            return false;
        };
    }
}
