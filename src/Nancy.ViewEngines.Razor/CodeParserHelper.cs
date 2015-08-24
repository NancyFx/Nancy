namespace Nancy.ViewEngines.Razor
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;

    using Nancy.Bootstrapper;

    internal static class CodeParserHelper
    {
        /// <summary>
        /// Throws exception says that given type was not found in any accessible assembly
        /// </summary>
        /// <param name="type">Type that was not found</param>
        public static void ThrowTypeNotFound(string type)
        {            
            throw new NotSupportedException(string.Format(
                "Unable to discover CLR Type for model by the name of {0}.\n\nTry using a fully qualified type name and ensure that the assembly is added to the configuration file.\n\nAppDomain Assemblies:\n\t{1}.\n\nCurrent ADATS assemblies:\n\t{2}.\n\nAssemblies in directories\n\t{3}",
                type,
                AppDomain.CurrentDomain.GetAssemblies().Select(a => a.FullName).Aggregate((n1, n2) => n1 + "\n\t" + n2),
                AppDomainAssemblyTypeScanner.Assemblies.Select(a => a.FullName).Aggregate((n1, n2) => n1 + "\n\t" + n2),
                GetAssembliesInDirectories().Aggregate((n1, n2) => n1 + "\n\t" + n2))
                );
        }

        private static IEnumerable<String> GetAssembliesInDirectories()
        {
            return GetAssemblyDirectories().SelectMany(d => Directory.GetFiles(d, "*.dll"));
        }

        /// <summary>
        /// Returns the directories containing dll files. It uses the default convention as stated by microsoft.
        /// </summary>
        /// <see cref="http://msdn.microsoft.com/en-us/library/system.appdomainsetup.privatebinpathprobe.aspx"/>
        private static IEnumerable<string> GetAssemblyDirectories()
        {            
            var privateBinPathDirectories = AppDomain.CurrentDomain.SetupInformation.PrivateBinPath == null
                                                ? new string[] { }
                                                : AppDomain.CurrentDomain.SetupInformation.PrivateBinPath.Split(';');

            foreach (var privateBinPathDirectory in privateBinPathDirectories)
            {
                if (!string.IsNullOrWhiteSpace(privateBinPathDirectory))
                {
                    yield return privateBinPathDirectory;
                }
            }

            if (AppDomain.CurrentDomain.SetupInformation.PrivateBinPathProbe == null)
            {
                yield return AppDomain.CurrentDomain.SetupInformation.ApplicationBase;
            }
        }
    }
}
