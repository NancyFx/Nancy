#if !CORE
namespace Nancy.Helpers
{
    using System;
    using System.Reflection;
    using Nancy.Extensions;

    internal class ProxyNancyReferenceProber : MarshalByRefObject
    {
        public bool HasReference(AssemblyName assemblyNameForProbing, AssemblyName referenceAssemblyName)
        {
            var assemblyForInspection = Assembly.Load(assemblyNameForProbing);

            return assemblyForInspection.IsReferencing(referenceAssemblyName);
        }
    }
}
#endif