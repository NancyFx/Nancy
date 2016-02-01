namespace Nancy
{
    using System;
    using System.Collections.Concurrent;
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
        private readonly Lazy<IReadOnlyCollection<Assembly>> assemblies = new Lazy<IReadOnlyCollection<Assembly>>(GetAssembliesInAppDomain);
        private readonly ConcurrentDictionary<AssemblyResolveStrategy, IReadOnlyCollection<Assembly>> cache;

        /// <summary>
        /// Initializes a new instance of the <see cref="AppDomainAssemblyCatalog"/> class.
        /// </summary>
        public AppDomainAssemblyCatalog()
        {
            this.cache = new ConcurrentDictionary<AssemblyResolveStrategy, IReadOnlyCollection<Assembly>>();

            EnsureNancyReferencingAssembliesAreLoaded();
        }

        /// <summary>
        /// Gets all <see cref="Assembly"/> instances in the catalog.
        /// </summary>
        /// <returns>An <see cref="IReadOnlyCollection{T}"/> of <see cref="Assembly"/> instances.</returns>
        public IReadOnlyCollection<Assembly> GetAssemblies(AssemblyResolveStrategy strategy)
        {
            return this.cache.GetOrAdd(strategy, s => this.assemblies.Value.Where(s.Invoke).ToArray());
        }

        private static IReadOnlyCollection<Assembly> GetAssembliesInAppDomain()
        {
            return AppDomain.CurrentDomain
                .GetAssemblies()
                .Where(assembly => !assembly.IsDynamic)
                .Where(assembly => !assembly.ReflectionOnly)
                .ToArray();
        }

        private static void EnsureNancyReferencingAssembliesAreLoaded()
        {
            var assemblyPaths = GetAssembliesInAppDomain()
                .Select(assembly => assembly.Location)
                .ToArray();

            foreach (var directory in GetAssemblyDirectories())
            {
                var unloadedAssemblies = Directory
                    .GetFiles(directory, "*.dll")
                    .Where(file => !assemblyPaths.Contains(file, StringComparer.OrdinalIgnoreCase))
                    .ToArray();

                foreach (var unloadedAssembly in unloadedAssemblies)
                {
                    var reflectionAssembly =
                        SafeLoadReflectionOnlyAssembly(unloadedAssembly);

                    if (reflectionAssembly == null)
                    {
                        continue;
                    }

                    if (!reflectionAssembly.GetReferencedAssemblies().Any(referece => referece.Name.Equals("Nancy", StringComparison.OrdinalIgnoreCase)))
                    {
                        continue;
                    }

                    SafeLoadAssemblyIntoApplicationDomain(reflectionAssembly);
                }
            }
        }

        private static void SafeLoadAssemblyIntoApplicationDomain(Assembly reflectionAssembly)
        {
            try
            {
                Assembly.Load(reflectionAssembly.GetName());
            }
            catch
            {
            }
        }

        private static Assembly SafeLoadReflectionOnlyAssembly(string assemblyPath)
        {
            try
            {
                return Assembly.ReflectionOnlyLoadFrom(assemblyPath);
            }
            catch (BadImageFormatException)
            {
                //the assembly maybe it's not managed assembly
            }
            catch (FileLoadException)
            {
                //the assembly might already be loaded
            }

            return null;
        }

        private static IEnumerable<string> GetAssemblyDirectories()
        {
            var directories = AppDomain.CurrentDomain.SetupInformation.PrivateBinPath != null
                ? AppDomain.CurrentDomain.SetupInformation.PrivateBinPath.Split(';')
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
    }
}
