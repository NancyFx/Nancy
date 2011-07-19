//namespace Nancy.ViewEngines
//{
//    using System;
//    using System.Collections.Generic;
//    using System.IO;
//    using System.Linq;
//    using System.Reflection;

//    using Nancy.Bootstrapper;

//    /// <summary>
//    /// Contains the functionality for locating a view that has been embedded into an assembly resource.
//    /// </summary>
//    public class ResourceViewLocationProviders : IViewLocationProvider
//    {
//        /// <summary>
//        /// Initializes a new instance of the <see cref="ResourceViewLocationProviders"/> class.
//        /// </summary>
//        public ResourceViewLocationProviders()
//        {
//        }

//        /// <summary>
//        /// Returns an <see cref="ViewLocationResult"/> instance for all the views that could be located by the provider.
//        /// </summary>
//        /// <param name="supportedViewExtensions">An <see cref="IEnumerable{T}"/> instance, containing the view engine file extensions that is supported by the running instance of Nancy.</param>
//        /// <returns>An <see cref="IEnumerable{T}"/> instance, containing <see cref="ViewLocationResult"/> instances for the located views.</returns>
//        /// <remarks>If no views could be located, this method should return an empty enumerable, never <see langword="null"/>.</remarks>
//        public IEnumerable<ViewLocationResult> GetLocatedViews(IEnumerable<string> supportedViewExtensions)
//        {
//            if (supportedViewExtensions == null)
//            {
//                return null;
//            }

//            var excludedAssemblies = new List<Func<Assembly, bool>>()
//            {
//                asm => asm.FullName.StartsWith("Microsoft.", StringComparison.InvariantCulture),
//                asm => asm.FullName.StartsWith("System.", StringComparison.InvariantCulture),
//                asm => asm.FullName.StartsWith("System,", StringComparison.InvariantCulture),
//                asm => asm.FullName.StartsWith("CR_ExtUnitTest", StringComparison.InvariantCulture),
//                asm => asm.FullName.StartsWith("mscorlib,", StringComparison.InvariantCulture),
//                asm => asm.FullName.StartsWith("CR_VSTest", StringComparison.InvariantCulture),
//                asm => asm.FullName.StartsWith("DevExpress.CodeRush", StringComparison.InvariantCulture),
//            };

//            var resourceStreamMatches = AppDomainAssemblyTypeScanner.Assemblies
//                .Where(x => !excludedAssemblies.Any(asm => asm.Invoke(x)))
//                .SelectMany(x => GetViewLocations(
//                    ExtractAssemblyCommonNamespace(x),
//                    GetResourceStreamMatches(x, supportedViewExtensions)));

//            return resourceStreamMatches;
//        }

//        private static IEnumerable<ViewLocationResult> GetViewLocations(string commonNameSpace, IList<Tuple<string, Stream>> resources)
//        {
//            return
//                from resource in resources
//                let resourceFileName = GetResourceFileName(resource.Item1)
//                where !resourceFileName.Equals(string.Empty)
//                select new ViewLocationResult(
//                    GetEncodedResouceName(commonNameSpace, resource.Item1, resourceFileName),
//                    resourceFileName,
//                    GetSafeResourceExtension(resource.Item1),
//                    () => new StreamReader(resource.Item2));
//        }

//        private static string GetEncodedResouceName(string commonNamespace, string location, string viewName)
//        {
//            if (commonNamespace.Equals(viewName, StringComparison.OrdinalIgnoreCase))
//            {
//                return viewName;
//            }

//            var locationWithoutViewName =
//                location.Replace(viewName, string.Empty);

//            var commonResourceNamespaceWithoutViewName =
//                commonNamespace.Replace(viewName, string.Empty);

//            var encodedLocation = locationWithoutViewName
//                .Replace(commonResourceNamespaceWithoutViewName, string.Empty)
//                .TrimEnd(new[] { '.' })
//                .Replace(".", "/");

//            if (encodedLocation.Length != 0)
//            {
//                encodedLocation = string.Concat(encodedLocation, @"\");
//            }

//            return string.Concat(encodedLocation, viewName);
//        }

//        private static string ExtractCommonResourceNamespace(IList<string> resources)
//        {
//            if (resources.Count() == 1)
//            {
//                var resource = resources.First();

//                return resource
//                    .Replace(GetResourceFileName(resource), string.Empty)
//                    .TrimEnd(new [] { '.' });
//            }

//            var commonPathSegments = resources.Select(s => new { parts = s.Split('.') })
//                .Aggregate((previous, current) => new { parts = current.parts.TakeWhile((step, index) => step == previous.parts.ElementAtOrDefault(index)).ToArray() });

//            var commonResourceNamespace =
//                string.Join(".", commonPathSegments.parts);

//            return commonResourceNamespace;
//        }

//        private static string GetResourceFileName(string resourceName)
//        {
//            var nameSegments =
//                resourceName.Split(new[] { "." }, StringSplitOptions.RemoveEmptyEntries);

//            var segmentCount =
//                nameSegments.Count();

//            return (segmentCount < 2) ?
//                string.Empty :
//                string.Concat(nameSegments[segmentCount - 2], ".", nameSegments[segmentCount - 1]);
//        }

//        private static string GetSafeResourceExtension(string resourceName)
//        {
//            var extension =
//                Path.GetExtension(resourceName);

//            return string.IsNullOrEmpty(extension) ? string.Empty : extension.TrimStart('.');
//        }

//        private static string ExtractAssemblyCommonNamespace(Assembly assembly)
//        {
//            var resources = assembly
//                .GetTypes()
//                .Where(x => !x.IsAnonymousType())
//                .Select(x => string.Concat(x.Namespace, ".", x.Name))
//                .ToList();

//            return ExtractCommonResourceNamespace(resources);
//        }

//        private static IList<Tuple<string, Stream>> GetResourceStreamMatches(Assembly assembly, IEnumerable<string> supportedViewEngineExtensions)
//        {
//            var resourceStreams =
//                from resourceName in assembly.GetManifestResourceNames()
//                from viewEngineExtension in supportedViewEngineExtensions
//                where resourceName.EndsWith(viewEngineExtension, StringComparison.OrdinalIgnoreCase)
//                select new Tuple<string, Stream>(resourceName, assembly.GetManifestResourceStream(resourceName));

//            return resourceStreams.ToList();
//        }
//    }
//}