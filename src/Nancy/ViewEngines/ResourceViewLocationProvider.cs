namespace Nancy.ViewEngines
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Reflection;

    /// <summary>
    /// Contains the functionality for locating a view that has been embedded into an assembly resource.
    /// </summary>
    public class ResourceViewLocationProvider : IViewLocationProvider
    {
        private readonly IResourceReader resourceReader;
        private readonly IResourceAssemblyProvider resourceAssemblyProvider;
        
        /// <summary>
        /// User-configured root namespaces for assemblies.
        /// </summary>
        public static IDictionary<Assembly, string> RootNamespaces = new Dictionary<Assembly, string>();
        
        /// <summary>
        /// A list of assemblies to ignore when scanning for embedded views.
        /// </summary>
        public static IList<Assembly> Ignore = new List<Assembly>();

        /// <summary>
        /// Initializes a new instance of the <see cref="ResourceViewLocationProvider"/> class.
        /// </summary>
        public ResourceViewLocationProvider()
            : this(new DefaultResourceReader(), new DefaultResourceAssemblyProvider())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ResourceViewLocationProvider"/> class.
        /// </summary>
        /// <param name="resourceReader">An <see cref="IResourceReader"/> instance that should be used when extracting embedded views.</param>
        /// <param name="resourceAssemblyProvider">An <see cref="IResourceAssemblyProvider"/> instance that should be used to determine which assemblies to scan for embedded views.</param>
        public ResourceViewLocationProvider(IResourceReader resourceReader, IResourceAssemblyProvider resourceAssemblyProvider)
        {
            this.resourceReader = resourceReader;
            this.resourceAssemblyProvider = resourceAssemblyProvider;
        }

        /// <summary>
        /// Returns an <see cref="ViewLocationResult"/> instance for all the views that could be located by the provider.
        /// </summary>
        /// <param name="supportedViewExtensions">An <see cref="IEnumerable{T}"/> instance, containing the view engine file extensions that is supported by the running instance of Nancy.</param>
        /// <returns>An <see cref="IEnumerable{T}"/> instance, containing <see cref="ViewLocationResult"/> instances for the located views.</returns>
        /// <remarks>If no views could be located, this method should return an empty enumerable, never <see langword="null"/>.</remarks>
        public IEnumerable<ViewLocationResult> GetLocatedViews(IEnumerable<string> supportedViewExtensions)
        {
            if (supportedViewExtensions == null || !supportedViewExtensions.Any())
            {
                return Enumerable.Empty<ViewLocationResult>();
            }

            return this.resourceAssemblyProvider
                .GetAssembliesToScan()
                .Where(x => !Ignore.Contains(x))
                .SelectMany(x => GetViewLocations(x, supportedViewExtensions));
        }

        private IEnumerable<ViewLocationResult> GetViewLocations(Assembly assembly, IEnumerable<string> supportedViewExtensions)
        {
            var resourceStreams = 
                this.resourceReader.GetResourceStreamMatches(assembly, supportedViewExtensions);

            if (!resourceStreams.Any())
            {
                return Enumerable.Empty<ViewLocationResult>();
            }

            if (resourceStreams.Count() == 1 && !RootNamespaces.ContainsKey(assembly))
            {
                var errorMessage =
                    string.Format("Only one view was found in assembly {0}, but no rootnamespace had been registered.", assembly.FullName);

                throw new InvalidOperationException(errorMessage);
            }

            var commonNamespace = RootNamespaces.ContainsKey(assembly) ?
                RootNamespaces[assembly] : 
                ExtractAssemblyRootNamespace(assembly);

            if (string.IsNullOrEmpty(commonNamespace))
            {
                return Enumerable.Empty<ViewLocationResult>();
            }

            return
                from resource in resourceStreams
                let resourceFileName = GetResourceFileName(resource.Item1)
                where !resourceFileName.Equals(string.Empty)
                select new ViewLocationResult(
                    GetResourceLocation(commonNamespace, resource.Item1, resourceFileName),
                    Path.GetFileNameWithoutExtension(resourceFileName),
                    GetResourceExtension(resource.Item1),
                    resource.Item2);
        }

        private static string GetResourceLocation(string commonNamespace, string resource, string resourceName)
        {
            return resource
                .Replace(commonNamespace, string.Empty)
                .Replace(resourceName, string.Empty)
                .Trim(new[] { '.' })
                .Replace(".", "/");
        }

        private static string ExtractCommonResourceNamespace(IEnumerable<string> resources)
        {
            if (resources.Count() == 1)
            {
                var resource = resources.First();

                return resource
                    .Replace(GetResourceFileName(resource), string.Empty)
                    .TrimEnd(new[] { '.' });
            }

            var commonPathSegments = resources.Select(s => new { parts = s.Split('.') })
                .Aggregate((previous, current) => new { parts = current.parts.TakeWhile((step, index) => step == previous.parts.ElementAtOrDefault(index)).ToArray() });

            var commonResourceNamespace =
                string.Join(".", commonPathSegments.parts);

            return commonResourceNamespace;
        }

        private static string ExtractAssemblyRootNamespace(Assembly assembly)
        {
            var resources = assembly
                .GetTypes()
                .Where(x => !x.IsAnonymousType())
                .Select(x => x.FullName)
                .ToList();

            return ExtractCommonResourceNamespace(resources);
        }

        private static string GetResourceFileName(string resourceName)
        {
            var nameSegments =
                resourceName.Split(new[] { "." }, StringSplitOptions.RemoveEmptyEntries);

            var segmentCount =
                nameSegments.Count();

            return (segmentCount < 2) ?
                string.Empty :
                string.Concat(nameSegments[segmentCount - 2], ".", nameSegments[segmentCount - 1]);
        }

        private static string GetResourceExtension(string resourceName)
        {
            return Path.GetExtension(resourceName).Substring(1);
        }
    }
}