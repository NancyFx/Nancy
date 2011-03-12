namespace Nancy.Bootstrapper
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Reflection;

    using Nancy.Extensions;

    /// <summary>
    /// Scans the app domain for types
    /// </summary>
    public static class AppDomainTypeScanner
    {
        /// <summary>
        /// App domain type cache
        /// </summary>
        private static IEnumerable<Type> types;

        /// <summary>
        /// Initializes static members of the <see cref="AppDomainTypeScanner"/> class.
        /// </summary>
        static AppDomainTypeScanner()
        {
            LoadNancyAssemblies();
        }

        /// <summary>
        /// Gets app domain types.
        /// </summary>
        public static IEnumerable<Type> Types
        {
            get
            {
                return types;
            }
        }

        /// <summary>
        /// Load assemblies from a the app domain base directory matching a given wildcard.
        /// Assemblies will only be loaded if they aren't already in the appdomain.
        /// </summary>
        /// <param name="wildcardFilename">Wildcard to match the assemblies to load</param>
        public static void LoadAssemblies(string wildcardFilename)
        {
            LoadAssemblies(AppDomain.CurrentDomain.BaseDirectory, wildcardFilename);
        }

        /// <summary>
        /// Load assemblies from a given directory matching a given wildcard.
        /// Assemblies will only be loaded if they aren't already in the appdomain.
        /// </summary>
        /// <param name="containingDirectory">Directory containing the assemblies</param>
        /// <param name="wildcardFilename">Wildcard to match the assemblies to load</param>
        public static void LoadAssemblies(string containingDirectory, string wildcardFilename)
        {
            var existingAssemblyPaths = AppDomain.CurrentDomain.GetAssemblies().Select(a => a.Location).ToArray();

            var unloadedNancyAssemblies =
                Directory.GetFiles(containingDirectory, wildcardFilename).Where(
                    f => !existingAssemblyPaths.Contains(f, StringComparer.InvariantCultureIgnoreCase));

            foreach (var unloadedNancyAssembly in unloadedNancyAssemblies)
            {
                Assembly.Load(AssemblyName.GetAssemblyName(unloadedNancyAssembly));
            }

            UpdateTypes();
        }

        /// <summary>
        /// Refreshes the type cache if additional assemblies have been loaded.
        /// Note: This is called automatically if assemblies are loaded using LoadAssemblies.
        /// </summary>
        public static void UpdateTypes()
        {
            types = (from assembly in AppDomain.CurrentDomain.GetAssemblies()
                     where !assembly.ReflectionOnly
                     where !assembly.IsDynamic
                     from type in assembly.SafeGetExportedTypes()
                     where !type.IsAbstract
                     select type).ToArray();
        }

        /// <summary>
        /// Loads any Nancy*.dll assemblies in the app domain base directory
        /// </summary>
        private static void LoadNancyAssemblies()
        {
            LoadAssemblies(@"Nancy*.dll");
        }
    }
}