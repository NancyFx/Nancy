namespace Nancy.Diagnostics
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Nancy.Bootstrapper;

    internal class DiagnosticsModuleCatalog : INancyModuleCatalog
    {
        private readonly IDictionary<string, Type> diagnosticModules;
 
        private readonly IModuleKeyGenerator keyGenerator;

        public DiagnosticsModuleCatalog(IModuleKeyGenerator keyGenerator)
        {
            this.keyGenerator = keyGenerator;

            var modules = AppDomainAssemblyTypeScanner.TypesOf<DiagnosticModule>().ToArray();

            this.diagnosticModules = new Dictionary<string, Type>(modules.Length);
            
            foreach (var module in modules)
            {
                this.diagnosticModules.Add(keyGenerator.GetKeyForModuleType(module), module);
            }
        }

        /// <summary>
        /// Get all NancyModule implementation instances - should be per-request lifetime
        /// </summary>
        /// <param name="context">The current context</param>
        /// <returns>An <see cref="IEnumerable{T}"/> instance containing <see cref="NancyModule"/> instances.</returns>
        public IEnumerable<NancyModule> GetAllModules(NancyContext context)
        {
            return this.diagnosticModules.Values.Select(t => (NancyModule)Activator.CreateInstance(t));
        }

        /// <summary>
        /// Retrieves a specific <see cref="NancyModule"/> implementation based on its key - should be per-request lifetime
        /// </summary>
        /// <param name="moduleKey">Module key</param>
        /// <param name="context">The current context</param>
        /// <returns>The <see cref="NancyModule"/> instance that was retrived by the <paramref name="moduleKey"/> parameter.</returns>
        public NancyModule GetModuleByKey(string moduleKey, NancyContext context)
        {
            return (NancyModule)Activator.CreateInstance(this.diagnosticModules[moduleKey]);
        }
    }
}