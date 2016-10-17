using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.AspNetCore.Mvc.Razor.Compilation;
using Microsoft.CodeAnalysis;
using Microsoft.Extensions.DependencyModel;

namespace Nancy.ViewEngines.Razor
{
    public class MetadataReferenceFeatureProvider : IApplicationFeatureProvider<MetadataReferenceFeature>
    {
        private readonly IRazorConfiguration configuration;
        private readonly IAssemblyCatalog assemblyCatalog;
        private readonly string[] defaultAssemblyDefinitions = {
            "mscorlib",
            "System.Private.CoreLib"
        };

        /// <summary>
        /// Initializes a new instance of the <see cref="MetadataReferenceFeatureProvider"/> class.
        /// </summary>
        /// <param name="configuration">An <see cref="IRazorConfiguration"/> instance.</param>
        /// <param name="assemblyCatalog">An <see cref="IAssemblyCatalog"/> instance.</param>
        public MetadataReferenceFeatureProvider(IRazorConfiguration configuration, IAssemblyCatalog assemblyCatalog)
        {
            this.configuration = configuration;
            this.assemblyCatalog = assemblyCatalog;
        }

        public void PopulateFeature(IEnumerable<ApplicationPart> parts, MetadataReferenceFeature feature)
        {
            if (parts == null)
            {
                throw new ArgumentNullException(nameof(parts));
            }

            if (feature == null)
            {
                throw new ArgumentNullException(nameof(feature));
            }

            foreach (Assembly assembly in GetAllAssemblies().Where(assembly => !assembly.IsDynamic && !string.IsNullOrEmpty(assembly.Location)))
            {
                feature.MetadataReferences.Add(MetadataReference.CreateFromFile(assembly.Location));
            }
        }

        private IReadOnlyCollection<Assembly> GetAllAssemblies()
        {
            return this.assemblyCatalog.GetAssemblies()
                .Union(this.LoadAssembliesInConfiguration())
                .Union(this.LoadDefaultAssemblies())
                .Union(this.LoadReferencedAssemblies())
                .ToArray();
        }

        private IEnumerable<Assembly> LoadAssembliesInConfiguration()
        {
            var loadedAssemblies = new HashSet<Assembly>();

            var validAssemblyDefinitions = this.configuration
                .GetAssemblyNames()
                .Where(definition => !string.IsNullOrEmpty(definition));

            foreach (var assemblyDefinition in validAssemblyDefinitions)
            {
                try
                {
                    loadedAssemblies.Add(Assembly.Load(new AssemblyName(assemblyDefinition)));
                }
                catch
                {
                }
            }

            return loadedAssemblies;
        }

        private IEnumerable<Assembly> LoadDefaultAssemblies()
        {
            var loadedAssemblies = new HashSet<Assembly>();

            foreach (var assemblyDefinition in this.defaultAssemblyDefinitions)
            {
                try
                {
                    loadedAssemblies.Add(Assembly.Load(new AssemblyName(assemblyDefinition)));
                }
                catch
                {
                }
            }

            return loadedAssemblies;
        }

        private IEnumerable<Assembly> LoadReferencedAssemblies()
        {
            var loadedAssemblies = new HashSet<Assembly>();

#if CORE
            foreach (var library in DependencyContext.Default.RuntimeLibraries)
            {
                try
                {
                    loadedAssemblies.Add(Assembly.Load(new AssemblyName(library.Name)));
                }
                catch
                {
                }
            }
#else
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                loadedAssemblies.Add(assembly);
            }
#endif

            return loadedAssemblies;
        }
    }
}