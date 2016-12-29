#if !CORE
namespace Nancy.Helpers
{
    using System;
    using System.Reflection;
    using Nancy.Extensions;

    /// <summary>
    /// Utility class used to probe assembly references.
    /// </summary>
    /// <remarks>
    /// Because this class inherits from <see cref="T:System.MarshalByRefObject"/> it can be used across different <see cref="T:System.AppDomain"/>.
    /// </remarks>
    internal class ProxyNancyReferenceProber : MarshalByRefObject
    {
        /// <summary>
        /// Determines if the assembly has a reference (dependency) upon another one.
        /// </summary>
        /// <param name="assemblyNameForProbing">The name of the assembly that will be tested.</param>
        /// <param name="referenceAssemblyName">The reference assembly name.</param>
        /// <returns>A boolean value indicating if there is a reference.</returns>
        public bool HasReference(AssemblyName assemblyNameForProbing, AssemblyName referenceAssemblyName)
        {
            var assemblyForInspection = Assembly.ReflectionOnlyLoad(assemblyNameForProbing.Name);

            return assemblyForInspection.IsReferencing(referenceAssemblyName);
        }
    }
}
#endif
