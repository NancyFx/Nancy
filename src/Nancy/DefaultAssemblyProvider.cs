namespace Nancy
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using Nancy.Bootstrapper;

    /// <summary>
    /// Default set of assemblies that should be scanned for views embedded as resources.
    /// </summary>
    public class DefaultAssemblyProvider : IAssemblyProvider
    {
        private IEnumerable<Assembly> filteredAssemblies;

        private readonly List<Func<Assembly, bool>> excludedAssemblies = new List<Func<Assembly, bool>>()
        {
            asm => asm.FullName.StartsWith("Microsoft.", StringComparison.InvariantCulture),
            asm => asm.FullName.StartsWith("System.", StringComparison.InvariantCulture),
            asm => asm.FullName.StartsWith("System,", StringComparison.InvariantCulture),
            asm => asm.FullName.StartsWith("CR_ExtUnitTest", StringComparison.InvariantCulture),
            asm => asm.FullName.StartsWith("mscorlib,", StringComparison.InvariantCulture),
            asm => asm.FullName.StartsWith("CR_VSTest", StringComparison.InvariantCulture),
            asm => asm.FullName.StartsWith("DevExpress.CodeRush", StringComparison.InvariantCulture),
            asm => asm.FullName.StartsWith("IronPython", StringComparison.InvariantCulture),
            asm => asm.FullName.StartsWith("IronRuby", StringComparison.InvariantCulture),
            asm => asm.FullName.StartsWith("xunit", StringComparison.InvariantCulture),
            asm => asm.FullName.StartsWith("Nancy.Testing", StringComparison.InvariantCulture),
            asm => asm.FullName.StartsWith("MonoDevelop.NUnit", StringComparison.InvariantCulture),
        };

        /// <summary>
        /// Gets a list of assemblies that should be scanned for views.
        /// </summary>
        /// <returns>An <see cref="IEnumerable{T}"/> of <see cref="Assembly"/> instances.</returns>
        public IEnumerable<Assembly> GetAssembliesToScan()
        {
            return (this.filteredAssemblies ?? (this.filteredAssemblies = GetFilteredAssemblies()));
        }

        private IEnumerable<Assembly> GetFilteredAssemblies()
        {
            return AppDomainAssemblyTypeScanner
                .Assemblies
                .Where(x => !x.IsDynamic)
                .Where(x => !this.excludedAssemblies.Any(asm => asm.Invoke(x)));
        }
    }
}