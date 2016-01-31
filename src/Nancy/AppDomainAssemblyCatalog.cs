namespace Nancy
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.IO;
    using System.Linq;
    using System.Reflection;

    /// <summary>
    /// Default implementation of the <see cref="IAssemblyCatalog"/> interface, based on
    /// retrieving <see cref="Assembly"/> information from <see cref="AppDomain.CurrentDomain"/>.
    /// </summary>
    public class AppDomainAssemblyCatalog : IAssemblyCatalog
    {
        private readonly Lazy<IReadOnlyCollection<Assembly>> assemblies = new Lazy<IReadOnlyCollection<Assembly>>(GetAllAssemblies);

        /// <summary>
        /// Gets all <see cref="Assembly"/> instances in the catalog.
        /// </summary>
        /// <returns>An <see cref="IReadOnlyCollection{T}"/> of <see cref="Assembly"/> instances.</returns>
        public IReadOnlyCollection<Assembly> GetAssemblies()
        {
            return this.assemblies.Value;
        }

        private static IReadOnlyCollection<Assembly> GetAllAssemblies()
        {
            EnsureNancyReferencingAssembliesAreLoaded();

            return GetAssembliesInAppDomain();
        }

        private static IReadOnlyCollection<Assembly> GetAssembliesInAppDomain()
        {
            var assemblies = AppDomain.CurrentDomain
                                      .GetAssemblies()
                                      .Where(IsNancyReferencingAssembly)
                                      .Where(assembly => !assembly.IsDynamic)
                                      .Where(assembly => !assembly.ReflectionOnly)
                                      .ToArray();

            return new ReadOnlyCollection<Assembly>(assemblies);
        }

        private static bool IsNancyReferencingAssembly(Assembly assembly)
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

                    if (!reflectionAssembly.GetReferencedAssemblies().Any(referece =>referece.Name.StartsWith("Nancy", StringComparison.OrdinalIgnoreCase)))
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
                //the assembly maybe it's not managed code
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
