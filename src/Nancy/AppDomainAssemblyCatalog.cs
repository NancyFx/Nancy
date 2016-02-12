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
    public class AppDomainAssemblyCatalog : AssemblyCatalogBase
    {
        private bool isAppDomainPrepouplated;

        /// <summary>
        /// Get all available <see cref="Assembly"/> instances.
        /// </summary>
        /// <returns>An <see cref="IReadOnlyCollection{T}"/> of <see cref="Assembly"/> instances.</returns>
        protected override IReadOnlyCollection<Assembly> GetAvailableAssemblies()
        {
            if (!this.isAppDomainPrepouplated)
            {
                this.PrepopulateAppDomain();
            }

            return AppDomain.CurrentDomain
                .GetAssemblies()
                .Where(assembly => !assembly.IsDynamic)
                .Where(assembly => !assembly.ReflectionOnly)
                .ToArray();
        }

        private void PrepopulateAppDomain()
        {
            this.isAppDomainPrepouplated = true;

            var assemblies = this.GetAvailableAssemblies();

            var assemblyPaths = assemblies
                .Select(assembly => assembly.Location)
                .ToArray();

            foreach (var assembly in assemblies)
            {
                var references = assembly.GetReferencedAssemblies();
                foreach (var reference in references)
                {
                    Assembly.Load(reference);
                }
            }

#if DNX
            // TEMPORARY FIX TO FUNCTION ON DNX UNTIL WE HAVE A PROPER IAssemblyCatalog implementation for
            // DNX. This NOT intendend to be published in a release verion of Nancy and will be replaced
            // asap. This ensures that the Nancy project, that we reference in our project.json (like
            // view engines) are loaded into the appdomain even though there's no reference to any of the
            // types in them. We can't rely on the existing code to load them.

            var libraryManager =
                Microsoft.Extensions.PlatformAbstractions.PlatformServices.Default.LibraryManager;

            var nancyLibraryName = typeof(INancyEngine).Assembly.GetName().Name;

            var referencing = libraryManager.GetReferencingLibraries(nancyLibraryName);
            foreach (var library in referencing)
            {
                Assembly.Load(library.Name);
            }
#endif

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
