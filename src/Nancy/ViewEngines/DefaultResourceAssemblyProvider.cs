namespace Nancy.ViewEngines
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using Bootstrapper;

    /// <summary>
    /// Default set of assemblies that should be scanned for views embedded as resources.
    /// </summary>
    public class DefaultResourceAssemblyProvider : IResourceAssemblyProvider
    {
        /// <summary>
        /// Gets a list of assemblies that should be scanned for views.
        /// </summary>
        /// <returns>An <see cref="IEnumerable{T}"/> of <see cref="Assembly"/> instances.</returns>
        public IEnumerable<Assembly> GetAssembliesToScan()
        {
            var excludedAssemblies = new List<Func<Assembly, bool>>()
            {
                asm => asm.FullName.StartsWith("Microsoft.", StringComparison.InvariantCulture),
                asm => asm.FullName.StartsWith("Microsoft,", StringComparison.InvariantCulture),
                asm => asm.FullName.StartsWith("System.", StringComparison.InvariantCulture),
                asm => asm.FullName.StartsWith("System,", StringComparison.InvariantCulture),
                asm => asm.FullName.StartsWith("CR_ExtUnitTest", StringComparison.InvariantCulture),
                asm => asm.FullName.StartsWith("mscorlib,", StringComparison.InvariantCulture),
                asm => asm.FullName.StartsWith("CR_VSTest", StringComparison.InvariantCulture),
                asm => asm.FullName.StartsWith("DevExpress.CodeRush", StringComparison.InvariantCulture),
            };

            return AppDomainAssemblyTypeScanner
                .Assemblies
                .Where(x => !excludedAssemblies.Any(asm => asm.Invoke(x)));
        }
    }
}