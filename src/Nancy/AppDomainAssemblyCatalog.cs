#if !CORE
namespace Nancy
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Reflection;

    /// <summary>
    /// Default implementation of the <see cref="IAssemblyCatalog"/> interface, based on
    /// retrieving <see cref="Assembly"/> information from <see cref="AppDomain.CurrentDomain"/>.
    /// </summary>
    public class AppDomainAssemblyCatalog : IAssemblyCatalog
    {
        private static readonly AssemblyName NancyAssemblyName = typeof(INancyEngine).GetTypeInfo().Assembly.GetName();
        private readonly Lazy<IReadOnlyCollection<Assembly>> assemblies = new Lazy<IReadOnlyCollection<Assembly>>(GetAvailableAssemblies);

        /// <summary>
        /// Gets all <see cref="Assembly"/> instances in the catalog.
        /// </summary>
        /// <returns>An <see cref="IReadOnlyCollection{T}"/> of <see cref="Assembly"/> instances.</returns>
        public IReadOnlyCollection<Assembly> GetAssemblies()
        {
            return this.assemblies.Value;
        }

        private static IReadOnlyCollection<Assembly> GetAvailableAssemblies()
        {
            var assemblies = GetLoadedNancyReferencingAssemblies();

            var loaded = LoadNancyReferencingAssemblies(assemblies);

            return assemblies.Union(loaded).ToArray();
        }

        private static List<Assembly> GetLoadedNancyReferencingAssemblies()
        {
            var assemblies = new List<Assembly>();

            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                if (!assembly.IsDynamic && !assembly.ReflectionOnly && IsNancyReferencing(assembly))
                {
                    assemblies.Add(assembly);
                }
            }

            return assemblies;
        }

        private static IEnumerable<Assembly> LoadNancyReferencingAssemblies(IEnumerable<Assembly> loadedAssemblies)
        {
            var assemblies = new HashSet<Assembly>();
            var inspectionAppDomain = AppDomain.CreateDomain("AppDomainAssemblyCatalog");
            var loadedNancyReferencingAssemblyNames = loadedAssemblies.Select(assembly => assembly.GetName()).ToArray();

            foreach (var directory in GetAssemblyDirectories())
            {
                foreach (var assemblyPath in Directory.EnumerateFiles(directory, "*.dll"))
                {
                    var unloadedAssemblyName = SafeGetAssemblyName(assemblyPath);

                    if (unloadedAssemblyName == null)
                    {
                        continue;
                    }

                    if (!loadedNancyReferencingAssemblyNames.Any(loadedNancyReferencingAssemblyName => AssemblyName.ReferenceMatchesDefinition(loadedNancyReferencingAssemblyName, unloadedAssemblyName)))
                    {
                        var inspectionAssembly = inspectionAppDomain.Load(unloadedAssemblyName);

                        if (IsNancyReferencing(inspectionAssembly))
                        {
                            var assembly = SafeLoadAssembly(unloadedAssemblyName);

                            if (assembly != null)
                            {
                                assemblies.Add(assembly);
                            }
                        }
                    }
                }
            }

            AppDomain.Unload(inspectionAppDomain);

            return assemblies.ToArray();
        }

        private static bool IsNancyReferencing(Assembly assembly)
        {
            if (AssemblyName.ReferenceMatchesDefinition(assembly.GetName(), NancyAssemblyName))
            {
                return true;
            }

            foreach (var referencedAssemblyName in assembly.GetReferencedAssemblies())
            {
                if (AssemblyName.ReferenceMatchesDefinition(referencedAssemblyName, NancyAssemblyName))
                {
                    return true;
                }
            }

            return false;
        }

        private static IEnumerable<string> GetAssemblyDirectories()
        {
            var directories = AppDomain.CurrentDomain.SetupInformation.PrivateBinPath != null
                ? AppDomain.CurrentDomain.SetupInformation.PrivateBinPath.Split(new [] { ';' }, StringSplitOptions.RemoveEmptyEntries)
                : new string[] { };

            foreach (var directory in directories.Where(directory => !string.IsNullOrWhiteSpace(directory)))
            {
                yield return directory;
            }

            if (AppDomain.CurrentDomain.SetupInformation.PrivateBinPathProbe == null)
            {
                yield return AppDomain.CurrentDomain.SetupInformation.ApplicationBase;
            }
        }

        private static AssemblyName SafeGetAssemblyName(string assemblyPath)
        {
            try
            {
                return AssemblyName.GetAssemblyName(assemblyPath);
            }
            catch
            {
                return null;
            }
        }

        private static Assembly SafeLoadAssembly(AssemblyName assemblyName)
        {
            try
            {
                return AppDomain.CurrentDomain.Load(assemblyName);
            }
            catch
            {
                return null;
            }
        }
    }
}
#endif